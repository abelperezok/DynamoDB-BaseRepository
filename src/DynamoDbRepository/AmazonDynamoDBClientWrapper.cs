using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbRepository
{
    public class AmazonDynamoDBClientWrapper
    {
        protected readonly IAmazonDynamoDB _dynamoDbClient;
        protected readonly string TableName;

        public AmazonDynamoDBClientWrapper(string tableName, string serviceUrl = null)
        {
            TableName = tableName;
            if (serviceUrl != null)
            {
                var config = new AmazonDynamoDBConfig { ServiceURL = serviceUrl };
                _dynamoDbClient = new AmazonDynamoDBClient(config);
            }
            else
            {
                _dynamoDbClient = new AmazonDynamoDBClient();
            }
        }

        private DynamoDBItem DynamoDBKey(string pk, string sk)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddPK(pk);
            dbItem.AddSK(sk);
            return dbItem;
        }



        public async Task PutItemAsync(string pk, string sk, DynamoDBItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var dbItemKey = DynamoDBKey(pk, sk);
            var dbItemData = dbItemKey.MergeData(item);

            var putItemRq = new PutItemRequest
            {
                TableName = TableName,
                Item = dbItemData.ToDictionary()
            };

            var result = await _dynamoDbClient.PutItemAsync(putItemRq);
        }


        public async Task DeleteItemAsync(string pkId, string skId)
        {
            var dbItemKey = DynamoDBKey(pkId, skId);
            var delItemRq = new DeleteItemRequest
            {
                TableName = TableName,
                Key = dbItemKey.ToDictionary()
            };
            var result = await _dynamoDbClient.DeleteItemAsync(delItemRq);
        }

        public async Task<DynamoDBItem> GetItemAsync(string pk, string sk)
        {
            var dbItemKey = DynamoDBKey(pk, sk);
            var getitemRq = new GetItemRequest
            {
                TableName = TableName,
                Key = dbItemKey.ToDictionary()
            };
            var getitemResponse = await _dynamoDbClient.GetItemAsync(getitemRq);
            return new DynamoDBItem(getitemResponse.Item);
        }





        // TODO: leaky abstraction here !!! QueryRequest
        public async Task<IList<DynamoDBItem>> QueryAsync(QueryRequest queryRequest)
        {
            var queryResponse = await _dynamoDbClient.QueryAsync(queryRequest);
            return queryResponse.Items.Select(x => new DynamoDBItem(x)).ToList();
        }

        // TODO: leaky abstraction here !!! QueryRequest
        public QueryRequest GetGSI1QueryRequest(string gsi1, string skPrefix)
        {
            return new QueryRequest
            {
                TableName = TableName,
                IndexName = DynamoDBConstants.GSI1,
                KeyConditionExpression = $"{DynamoDBConstants.GSI1} = :gsi1_value and begins_with({DynamoDBConstants.SK}, :sk_prefix)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":gsi1_value", new AttributeValue(gsi1) },
                    { ":sk_prefix", new AttributeValue(skPrefix) }
                }
            };
        }

        // TODO: leaky abstraction here !!! QueryRequest
        public QueryRequest GetTableQueryRequest(string pk, string skPrefix)
        {
            return new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = $"{DynamoDBConstants.PK} = :pk_value and begins_with({DynamoDBConstants.SK}, :sk_prefix)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk_value", new AttributeValue(pk) },
                    { ":sk_prefix", new AttributeValue(skPrefix) }
                }
            };
        }
    }
}