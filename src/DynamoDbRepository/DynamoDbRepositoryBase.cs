using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbRepository
{
    public abstract class DynamoDbRepositoryBase
    {
        protected readonly IAmazonDynamoDB _dynamoDbClient;
        protected readonly string _tableName;
        protected readonly string Separator = "#";
        protected readonly string PK = "PK";
        protected readonly string SK = "SK";
        protected readonly string GSI1 = "GSI1";
        protected string PKPrefix = "";
        protected string SKPrefix = "";

        public DynamoDbRepositoryBase(string tableName)
        {
            _tableName = tableName;
            _dynamoDbClient = new AmazonDynamoDBClient();
        }

        protected string GetStringAttributeValue(string key, Dictionary<string, AttributeValue> item)
        {
            return item.GetValueOrDefault(key)?.S;
        }

        protected int GetNumberInt32AttributeValue(string key, Dictionary<string, AttributeValue> item)
        {
            return Convert.ToInt32(item.GetValueOrDefault(key)?.N);
        }

        protected double GetNumberDoubleAttributeValue(string key, Dictionary<string, AttributeValue> item)
        {
            return Convert.ToDouble(item.GetValueOrDefault(key)?.N);
        }

        protected QueryRequest GetAllQueryGSIRequest()
        {
            return new QueryRequest
            {
                TableName = _tableName,
                IndexName = GSI1,
                KeyConditionExpression = $"{GSI1} = :pk_prefix",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":pk_prefix", new AttributeValue(PKPrefix) } }
            };
        }

        protected AttributeValue PKAttributeValue(object id)
        {
            return new AttributeValue(PKPrefix + Separator + Convert.ToString(id));
        }

        protected AttributeValue SKAttributeValue(object id)
        {
            return new AttributeValue(SKPrefix + Separator + Convert.ToString(id));
        }

        protected AttributeValue StringAttributeValue(string value)
        {
            return new AttributeValue(value);
        }

        protected AttributeValue NumberAttributeValue(int value)
        {
            return BaseNumberAttributeValue(Convert.ToString(value));
        }

        protected AttributeValue NumberAttributeValue(double value)
        {
            return BaseNumberAttributeValue(Convert.ToString(value));
        }



        private AttributeValue BaseNumberAttributeValue(string value)
        {
            var result = new AttributeValue();
            result.N = value;
            return result;
        }
    }
}