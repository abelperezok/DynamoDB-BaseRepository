using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class PersonRepository : DynamoDbRepository<int, Person>
    {
        public PersonRepository(string tableName) : base(tableName)
        {
            PKPrefix = "PERSON";
            SKPrefix = "METADATA";
        }

        protected override Person FromDynamoDb(Dictionary<string, AttributeValue> item)
        {
            var result = new Person();
            result.Id = GetNumberInt32AttributeValue("Id", item);
            result.Name = GetStringAttributeValue("Name", item);
            result.FirstName = GetStringAttributeValue("FirstName", item);
            result.LastName = GetStringAttributeValue("LastName", item);
            result.Email = GetStringAttributeValue("Email", item);
            return result;
        }

        protected override Dictionary<string, AttributeValue> ToDynamoDb(Person item)
        {
            var dbItem = new Dictionary<string, AttributeValue>();
            dbItem.Add(PK, PKAttributeValue(item.Id));
            dbItem.Add(SK, SKAttributeValue(item.Id));

            dbItem.Add("Id", NumberAttributeValue(item.Id));
            dbItem.Add("Name", StringAttributeValue(item.Name));
            dbItem.Add("FirstName", StringAttributeValue(item.FirstName));
            dbItem.Add("LastName", StringAttributeValue(item.LastName));
            dbItem.Add("Email", StringAttributeValue(item.Email));
            // for GSI query all 
            dbItem.Add(GSI1, StringAttributeValue(PKPrefix));
            return dbItem;
        }

        protected override Dictionary<string, AttributeValue> ToDynamoDbPrimaryKey(int id)
        {
            return new Dictionary<string, AttributeValue>
                {
                    { PK, PKAttributeValue(id) },
                    { SK, SKAttributeValue(id) },
                };
        }
    }
}