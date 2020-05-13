using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbRepository
{
    public abstract class DynamoDbRepository<TKey, TEntity> : DynamoDbRepositoryBase
        where TEntity : class
    {
        public DynamoDbRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
        }

        protected abstract TKey GetEntityKey(TEntity item);

        protected abstract Dictionary<string, AttributeValue> ToDynamoDb(TEntity item);

        protected abstract TEntity FromDynamoDb(Dictionary<string, AttributeValue> item);

        protected virtual Dictionary<string, AttributeValue> ToDynamoDbPrimaryKey(TKey pkId, TKey skId)
        {
            var pk = (EqualityComparer<TKey>.Default.Equals(pkId, default(TKey))) ? skId : pkId;
            return new Dictionary<string, AttributeValue>
            {
                { PK, PKAttributeValue(pk) },
                { SK, SKAttributeValue(skId) },
            };
        }
        
        protected virtual Dictionary<string, AttributeValue> ToDynamoDbFullItem(TKey pkId, TEntity item)
        {
            var skId = GetEntityKey(item);

            var dbItemBase = ToDynamoDbPrimaryKey(pkId, skId);

            var dbItemData = ToDynamoDb(item);

            if ((EqualityComparer<TKey>.Default.Equals(pkId, default(TKey))))
            {
                dbItemBase.Add(GSI1, StringAttributeValue(PKPrefix));
            }

            return dbItemBase.Union(dbItemData).ToDictionary(k => k.Key, v => v.Value);
        }


        public async Task<TEntity> GetItemAsync(TKey id)
        {
            return await GetItemAsync(id, id);
        }

        public async Task<TEntity> GetItemAsync(TKey pkId, TKey skId)
        {
            var getitemRq = new GetItemRequest
            {
                TableName = _tableName,
                Key = ToDynamoDbPrimaryKey(pkId, skId)
            };
            var getitemResponse = await _dynamoDbClient.GetItemAsync(getitemRq);
            var result = getitemResponse.Item;

            if (result.Count > 0)
                return FromDynamoDb(result);

            return default(TEntity);
        }

        public async Task<IList<TEntity>> GetItemsByParentIdAsync(TKey pkId)
        {
            var queryRq = GetItemsByParentIdQueryTableRequest(pkId);
            var queryResponse = await _dynamoDbClient.QueryAsync(queryRq);
            var result = queryResponse.Items;
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<IList<TEntity>> GetAllItemsAsync()
        {
            var queryRq = GetAllQueryGSIRequest();
            var queryResponse = await _dynamoDbClient.QueryAsync(queryRq);
            var result = queryResponse.Items;
            return result.Select(FromDynamoDb).ToList();
        }

        public async Task<int> CountAsync()
        {
            var queryRq = GetAllQueryGSIRequest();
            queryRq.Select = Select.COUNT;

            var queryResponse = await _dynamoDbClient.QueryAsync(queryRq);
            return queryResponse.Count;
        }



        public async Task AddItemAsync(TEntity item)
        {
            await AddItemAsync(default(TKey), item);
        }

        public async Task AddItemAsync(TKey pkId, TEntity item)
        {
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDbFullItem(pkId, item)
            };

            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;

        }

        public async Task BatchAddItemAsync(IEnumerable<TEntity> items)
        {
            await BatchAddItemAsync(default(TKey), items);
        }

        public async Task BatchAddItemAsync(TKey pkId, IEnumerable<TEntity> items)
        {
            var requests = new List<WriteRequest>();
            foreach (var item in items)
            {
                var putRq = new PutRequest(ToDynamoDbFullItem(pkId, item));
                requests.Add(new WriteRequest(putRq));
            }

            var batchRq = new Dictionary<string, List<WriteRequest>> { { _tableName, requests } };

            var result = await _dynamoDbClient.BatchWriteItemAsync(batchRq);
        }

        public async Task UpdateItemAsync(TEntity item)
        {
            await UpdateItemAsync(default(TKey), item);
        }

        public async Task UpdateItemAsync(TKey pkId, TEntity item)
        {
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDbFullItem(pkId, item)
            };
            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
        }

        public async Task DeleteItemAsync(TKey id)
        {
            await DeleteItemAsync(id, id);
        }

        public async Task DeleteItemAsync(TKey pkId, TKey skId)
        {
            var delItemRq = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = ToDynamoDbPrimaryKey(pkId, skId)
            };
            var result = await _dynamoDbClient.DeleteItemAsync(delItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task BatchDeleteItemsAsync(IEnumerable<TEntity> items)
        {
            await BatchDeleteItemsAsync(default(TKey), items);
        }

        public async Task BatchDeleteItemsAsync(TKey pkId, IEnumerable<TEntity> items)
        {
            var requests = new List<WriteRequest>();
            foreach (var item in items)
            {
                var skId = GetEntityKey(item);
                var deleteRq = new DeleteRequest(ToDynamoDbPrimaryKey(pkId, skId));
                requests.Add(new WriteRequest(deleteRq));
            }

            var batchRq = new Dictionary<string, List<WriteRequest>> { { _tableName, requests } };

            var result = await _dynamoDbClient.BatchWriteItemAsync(batchRq);
        }
    }
}