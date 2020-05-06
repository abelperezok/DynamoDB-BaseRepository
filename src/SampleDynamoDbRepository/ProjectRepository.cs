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

        protected override Dictionary<string, AttributeValue> ToDynamoDb(Project item)
        {
            var dbItem = new Dictionary<string, AttributeValue>();
            dbItem.Add(PK, PKAttributeValue(item.Id));
            dbItem.Add(SK, SKAttributeValue(item.Id));

            dbItem.Add("Id", StringAttributeValue(item.Id));
            dbItem.Add("Name", StringAttributeValue(item.Name));
            dbItem.Add("Description", StringAttributeValue(item.Description));
            // for GSI query all 
            dbItem.Add(GSI1, StringAttributeValue(PKPrefix));
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