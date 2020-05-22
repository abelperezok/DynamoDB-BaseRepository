using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbRepository
{
    public abstract class AssociativeEntityRepository<TKey, TEntity> : RepositoryBase<TKey, TEntity>
            where TEntity : class
    {
        public AssociativeEntityRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
        }

        protected abstract TKey GetRelationKey(TKey parent1Key, TKey parent2Key);

        public async Task AddItemAsync(TKey parent1Key, TKey parent2Key, TEntity item)
        {
            var relationKey = GetRelationKey(parent1Key, parent2Key);
            var pk = PKValue(parent1Key);
            var sk = SKValue(relationKey);
            var dbItem = ToDynamoDb(item);
            // TODO: try to make the value from dbItem to take precedence 
            dbItem.AddGSI1(GSI1Value(parent2Key));
            await _dynamoDbClient.PutItemAsync(pk, sk, dbItem);
        }

        public async Task DeleteItemAsync(TKey parent1Key, TKey parent2Key)
        {
            var relationKey = GetRelationKey(parent1Key, parent2Key);
            var pk = PKValue(parent1Key);
            var sk = SKValue(relationKey);
            await _dynamoDbClient.DeleteItemAsync(pk, sk);
        }


        public async Task<IList<TEntity>> GSI1QueryItemsByParentIdAsync(TKey parentKey)
        {
            var gsi1 = GSI1Value(parentKey);
            var queryRq = _dynamoDbClient.GetGSI1QueryRequest(gsi1, SKPrefix);

            var items = await _dynamoDbClient.QueryAsync(queryRq);
            return items.Select(FromDynamoDb).ToList();
        }

        public async Task<IList<TEntity>> TableQueryItemsByParentIdAsync(TKey parentKey)
        {
            var pk = PKValue(parentKey);
            var queryRq = _dynamoDbClient.GetTableQueryRequest(pk, SKPrefix);
            var result = await _dynamoDbClient.QueryAsync(queryRq);
            return result.Select(FromDynamoDb).ToList();
        }
    }
}