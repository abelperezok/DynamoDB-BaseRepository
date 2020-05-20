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
            var pk = PKValue(parentKey);
            var sk = SKValue(entityKey);
            var dbItem = ToDynamoDb(item);
            await _dynamoDbClient.PutItemAsync(pk, sk, dbItem);
        }

        public async Task<IList<TEntity>> TableQueryItemsByParentIdAsync(TKey parentKey)
        {
            var pk = PKValue(parentKey);
            var queryRq = _dynamoDbClient.GetTableQueryRequest(pk, SKPrefix);
            var result = await _dynamoDbClient.QueryAsync(queryRq);
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<TEntity> GetItemAsync(TKey parentKey, TKey entityKey)
        {
            var pk = PKValue(parentKey);
            var sk = SKValue(entityKey);
            var item = await _dynamoDbClient.GetItemAsync(pk, sk);
            if (item.IsEmpty)
                return default(TEntity);
            return FromDynamoDb(item);
        }

        public async Task<IList<TEntity>> GSI1QueryItemsByParentIdAsync(TKey parentKey)
        {
            var gsi1 = GSI1Value(parentKey);
            var queryRq = _dynamoDbClient.GetGSI1QueryRequest(gsi1, SKPrefix);

            var items = await _dynamoDbClient.QueryAsync(queryRq);
            return items.Select(FromDynamoDb).ToList();
        }

        public async Task DeleteItemAsync(TKey parentKey, TKey entityKey)
        {
            var pk = PKValue(parentKey);
            var sk = SKValue(entityKey);
            await _dynamoDbClient.DeleteItemAsync(pk, sk);
        }

        public async Task BatchAddItemsAsync(TKey parentKey, IEnumerable<KeyValuePair<TKey, TEntity>> items)
        {
            var pk = PKValue(parentKey);
            var dbItems = new List<DynamoDBItem>();
            foreach (var item in items)
            {
                var sk = SKValue(item.Key);
                var dbItem = ToDynamoDb(item.Value);
                dbItem.AddPK(pk);
                dbItem.AddSK(sk);

                dbItems.Add(dbItem);
            }

            await _dynamoDbClient.BatchAddItemsAsync(dbItems);
        }

        public async Task BatchDeleteItemsAsync(TKey parentKey, IEnumerable<TKey> items)
        {
            var pk = PKValue(parentKey);
            var dbItems = new List<DynamoDBItem>();
            foreach (var item in items)
            {
                var dbItem = new DynamoDBItem();
                dbItem.AddPK(pk);
                dbItem.AddSK(SKValue(item));

                dbItems.Add(dbItem);
            }

            await _dynamoDbClient.BatchDeleteItemsAsync(dbItems);
        }
    }
}