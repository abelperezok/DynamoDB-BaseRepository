using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbRepository
{
    public abstract class DependentEntityRepository<TKey, TEntity> : RepositoryBase<TKey, TEntity>
        where TEntity : class
    {
        public DependentEntityRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
        }

        public async Task AddItemAsync(TKey parentKey, TKey entityKey, TEntity item)
        {
            var pk = PKValue(Convert.ToString(parentKey));
            var sk = SKValue(Convert.ToString(entityKey));
            var dbItem = ToDynamoDb(item);
            await _dynamoDbClient.PutItemAsync(pk, sk, dbItem);
        }

        public async Task<IList<TEntity>> TableQueryItemsByParentIdAsync(TKey parentKey)
        {
            var pk = PKValue(Convert.ToString(parentKey));
            var queryRq = _dynamoDbClient.GetTableQueryRequest(pk, SKPrefix);
            var result = await _dynamoDbClient.QueryAsync(queryRq);
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<TEntity> GetItemAsync(TKey parentKey, TKey entityKey)
        {
            var pk = PKValue(Convert.ToString(parentKey));
            var sk = SKValue(Convert.ToString(entityKey));
            var item = await _dynamoDbClient.GetItemAsync(pk, sk);
            if (item.IsEmpty)
                return default(TEntity);
            return FromDynamoDb(item);
        }

        public async Task<IList<TEntity>> GSI1QueryItemsByParentIdAsync(TKey parentKey)
        {
            var gsi1 = GSI1Value(Convert.ToString(parentKey));
            var queryRq = _dynamoDbClient.GetGSI1QueryRequest(gsi1, SKPrefix);

            var items = await _dynamoDbClient.QueryAsync(queryRq);
            return items.Select(FromDynamoDb).ToList();
        }

        public async Task DeleteItemAsync(TKey parentKey, TKey entityKey)
        {
            var pk = PKValue(Convert.ToString(parentKey));
            var sk = SKValue(Convert.ToString(entityKey));
            await _dynamoDbClient.DeleteItemAsync(pk, sk);
        }
    }
}