using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoCode.Domain.Data.Interfaces;
using DynamoCode.Domain.Entities;

namespace DynamoDbRepository
{
    public abstract class DynamoDbRepository<TKey, TEntity> : DynamoDbRepositoryBase, IAsyncRepository<TKey, TEntity> 
        where TEntity : class,IEntityKey<TKey> 
    {
        public DynamoDbRepository(string tableName) : base(tableName)
        {
        }

        protected abstract Dictionary<string, AttributeValue> ToDynamoDb(TEntity item);

        protected abstract TEntity FromDynamoDb(Dictionary<string, AttributeValue> item);

        protected abstract Dictionary<string, AttributeValue> ToDynamoDbPrimaryKey(TKey id);


        #region [    IAsyncReadRepository interface    ]

        public async Task<TEntity> FindByAsync(TKey id)
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

        public async Task<IList<TEntity>> AllAsync()
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

        #endregion


        #region [    IAsyncWriteRepository interface    ]

        public async Task AddAsync(TEntity item)
        {
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDb(item)
            };
            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;

        }

        public async Task AddAsync(IEnumerable<TEntity> items)
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

        public async Task UpdateAsync(TEntity item)
        {
            var putItemRq = new PutItemRequest
            {
                TableName = _tableName,
                Item = ToDynamoDb(item)
            };
            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
        }

        public async Task DeleteAsync(TEntity item)
        {
            await DeleteAsync(item.Id);
        }

        public async Task DeleteAsync(TKey id)
        {
            var delItemRq = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = ToDynamoDbPrimaryKey(id)
            };
            var result = await _dynamoDbClient.DeleteItemAsync(delItemRq);
            //return result.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task DeleteAsync(IEnumerable<TEntity> items)
        {
            var requests = new List<WriteRequest>();
            foreach (var item in items)
            {
                var deleteRq = new DeleteRequest(ToDynamoDbPrimaryKey(item.Id));
                requests.Add(new WriteRequest(deleteRq));
            }

            var batchRq = new Dictionary<string, List<WriteRequest>> { { _tableName, requests } };

            var result = await _dynamoDbClient.BatchWriteItemAsync(batchRq);
        }

        #endregion
    }
}