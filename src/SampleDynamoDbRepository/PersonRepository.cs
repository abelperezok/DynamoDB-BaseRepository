using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class PersonRepository : DynamoDbRepository<int, Person>
    {
        public PersonRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "PERSON";
            SKPrefix = "METADATA";
        }

        protected override int GetEntityKey(Person item)
        {
            return item.Id;
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
            var dbItem = base.ToDynamoDb(item);

            dbItem.Add("Id", NumberAttributeValue(item.Id));
            dbItem.Add("Name", StringAttributeValue(item.Name));
            dbItem.Add("FirstName", StringAttributeValue(item.FirstName));
            dbItem.Add("LastName", StringAttributeValue(item.LastName));
            dbItem.Add("Email", StringAttributeValue(item.Email));

            return dbItem;
        }
    }
}