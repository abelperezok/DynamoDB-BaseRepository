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

        protected virtual Dictionary<string, AttributeValue> ToDynamoDb(TEntity item)
        {
            var key = GetEntityKey(item);
            var dbItem = ToDynamoDbPrimaryKey(key);
            dbItem.Add(GSI1, StringAttributeValue(PKPrefix));
            return dbItem;
        }

        protected abstract TEntity FromDynamoDb(Dictionary<string, AttributeValue> item);

        protected virtual Dictionary<string, AttributeValue> ToDynamoDbPrimaryKey(TKey id)
        {
            return new Dictionary<string, AttributeValue>
            {
                { PK, PKAttributeValue(id) },
                { SK, SKAttributeValue(id) },
            };
        }



        public async Task<TEntity> GetItemAsync(TKey id)
        {
            var getitemRq = new GetItemRequest
            {
                TableName = _tableName,
                Key = ToDynamoDbPrimaryKey(id)
            };
            var getitemResponse = await _dynamoDbClient.GetItemAsync(getitemRq);
            var result = getitemResponse.Item;

            if (result.Count > 0)
                return FromDynamoDb(result);

            return default(TEntity);
        }

        public async Task<IList<TEntity>> GetAllAsync()
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
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDb(item)
            };
            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;

        }

        public async Task BatchAddItemAsync(IEnumerable<TEntity> items)
        {
            var requests = new List<WriteRequest>();
            foreach (var item in items)
            {
                var putRq = new PutRequest(ToDynamoDb(item));
                requests.Add(new WriteRequest(putRq));
            }

            var batchRq = new Dictionary<string, List<WriteRequest>> { { _tableName, requests } };

            var result = await _dynamoDbClient.BatchWriteItemAsync(batchRq);
        }

        public async Task UpdateItemAsync(TEntity item)
        {
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDb(item)
            };
            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
        }


        public async Task DeleteItemAsync(TKey id)
        {
            var delItemRq = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = ToDynamoDbPrimaryKey(id)
            };
            var result = await _dynamoDbClient.DeleteItemAsync(delItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task BatchDeleteItemsAsync(IEnumerable<TEntity> items)
        {
            var requests = new List<WriteRequest>();
            foreach (var item in items)
            {
                var key = GetEntityKey(item);
                var deleteRq = new DeleteRequest(ToDynamoDbPrimaryKey(key));
                requests.Add(new WriteRequest(deleteRq));
            }

            var batchRq = new Dictionary<string, List<WriteRequest>> { { _tableName, requests } };

            var result = await _dynamoDbClient.BatchWriteItemAsync(batchRq);
        }
    }
}