using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class UserRepository : DynamoDbRepository<string, User>
    {
        public UserRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "METADATA";
        }

        protected override Dictionary<string, AttributeValue> ToDynamoDb(User item)
        {
            var dbItem = new Dictionary<string, AttributeValue>();
            dbItem.Add(PK, PKAttributeValue(item.Id));
            dbItem.Add(SK, SKAttributeValue(item.Id));

            dbItem.Add("Id", StringAttributeValue(item.Id));
            dbItem.Add("Name", StringAttributeValue(item.Name));
            dbItem.Add("FirstName", StringAttributeValue(item.FirstName));
            dbItem.Add("LastName", StringAttributeValue(item.LastName));
            dbItem.Add("Email", StringAttributeValue(item.Email));
            // for GSI query all 
            dbItem.Add(GSI1, StringAttributeValue(PKPrefix));
            return dbItem;
        }

        protected override User FromDynamoDb(Dictionary<string, AttributeValue> item)
        {
            var result = new User();
            result.Id = GetStringAttributeValue("Id", item);
            result.Name = GetStringAttributeValue("Name", item);
            result.FirstName = GetStringAttributeValue("FirstName", item);
            result.LastName = GetStringAttributeValue("LastName", item);
            result.Email = GetStringAttributeValue("Email", item);
            return result;
        }
    }
}