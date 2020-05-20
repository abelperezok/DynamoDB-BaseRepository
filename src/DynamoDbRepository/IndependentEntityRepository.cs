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
            var pk = PKValue(key);
            var sk = SKValue(key);
            var dbItem = ToDynamoDb(item);
            // TODO: try to make the value from dbItem to take precedence 
            dbItem.AddGSI1(PKPrefix);
            await _dynamoDbClient.PutItemAsync(pk, sk, dbItem);
        }

        public async Task<IList<TEntity>> GSI1QueryAllAsync()
        {
            var queryRq = _dynamoDbClient.GetGSI1QueryRequest(PKPrefix, SKPrefix);
            var result = await _dynamoDbClient.QueryAsync(queryRq);
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<TEntity> GetItemAsync(TKey key)
        {
            var pk = PKValue(key);
            var sk = SKValue(key);
            var item = await _dynamoDbClient.GetItemAsync(pk, sk);
            if (item.IsEmpty)
                return default(TEntity);
            return FromDynamoDb(item);
        }

        public async Task DeleteItemAsync(TKey key)
        {
            var pk = PKValue(key);
            var sk = SKValue(key);
            await _dynamoDbClient.DeleteItemAsync(pk, sk);
        }

        public async Task BatchAddItemsAsync(IEnumerable<KeyValuePair<TKey, TEntity>> items)
        {
            var dbItems = new List<DynamoDBItem>();
            foreach (var item in items)
            {
                var dbItem = ToDynamoDb(item.Value);
                dbItem.AddPK(PKValue(item.Key));
                dbItem.AddSK(SKValue(item.Key));
                // TODO: try to make the value from dbItem to take precedence 
                dbItem.AddGSI1(PKPrefix);

                dbItems.Add(dbItem);
            }

            await _dynamoDbClient.BatchAddItemsAsync(dbItems);
        }

        public async Task BatchDeleteItemsAsync(IEnumerable<TKey> items)
        {
            var dbItems = new List<DynamoDBItem>();
            foreach (var item in items)
            {
                var dbItem = new DynamoDBItem();
                dbItem.AddPK(PKValue(item));
                dbItem.AddSK(SKValue(item));

                dbItems.Add(dbItem);
            }

            await _dynamoDbClient.BatchDeleteItemsAsync(dbItems);
        }
    }
}