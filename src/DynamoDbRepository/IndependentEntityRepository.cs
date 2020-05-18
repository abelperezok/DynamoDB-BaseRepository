using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbRepository
{
    public abstract class IndependentEntityRepository<TKey, TEntity> : RepositoryBase<TKey, TEntity>
        where TEntity : class
    {

        public IndependentEntityRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
        }

        public async Task AddItemAsync(TKey key, TEntity item)
        {
            var pk = PKValue(Convert.ToString(key));
            var sk = SKValue(Convert.ToString(key));
            var dbItem = ToDynamoDb(item);
            // TODO: try to make the value from dbItem to take precedence 
            dbItem.AddGSI1Value(PKPrefix);
            await _dynamoDbClient.PutItemAsync(pk, sk, dbItem);
        }

        public async Task<IList<TEntity>> GSI1QueryAllAsync()
        {
            var queryRq = _dynamoDbClient.GetGSI1AllQueryRequest(PKPrefix, SKPrefix);
            var result = await _dynamoDbClient.QueryAsync(queryRq);
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<TEntity> GetItemAsync(TKey key)
        {
            var pk = PKValue(Convert.ToString(key));
            var sk = SKValue(Convert.ToString(key));
            var item = await _dynamoDbClient.GetItemAsync(pk, sk);
            if (item.IsEmpty)
                return default(TEntity);
            return FromDynamoDb(item);
        }

        public async Task DeleteItemAsync(TKey key)
        {
            var pk = PKValue(Convert.ToString(key));
            var sk = SKValue(Convert.ToString(key));
            await _dynamoDbClient.DeleteItemAsync(pk, sk);
        }
    }
}