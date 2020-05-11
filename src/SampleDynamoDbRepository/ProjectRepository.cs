using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class ProjectRepository : DynamoDbRepository<string, Project>
    {
        public ProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "PROJECT";
            SKPrefix = "METADATA";
        }

        protected override string GetEntityKey(Project item)
        {
            return item.Id;
        }

        protected override Dictionary<string, AttributeValue> ToDynamoDb(Project item)
        {
            var dbItem = base.ToDynamoDb(item);

            dbItem.Add("Id", StringAttributeValue(item.Id));
            dbItem.Add("Name", StringAttributeValue(item.Name));
            dbItem.Add("Description", StringAttributeValue(item.Description));
            
            return dbItem;
        }

        protected override Project FromDynamoDb(Dictionary<string, AttributeValue> item)
        {
            var result = new Project();
            result.Id = GetStringAttributeValue("Id", item);
            result.Name = GetStringAttributeValue("Name", item);
            result.Description = GetStringAttributeValue("Description", item);
            return result;
        }
    }
}