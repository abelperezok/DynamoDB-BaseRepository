using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class UserProjectRepository : DynamoDbRepository<string, UserProject>
    {

        public UserProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "USER_PROJECT";
            GSIPrefix = "PROJECT";
        }

        protected override string GetEntityKey(UserProject item)
        {
            return item.UserId;
        }

        protected override string GetEntitySortKey(UserProject item)
        {
            return item.UserId + item.ProjectId;
        }

        protected override Dictionary<string, AttributeValue> ToDynamoDb(UserProject item)
        {
            var dbItem =  new Dictionary<string, AttributeValue>();            
            
            dbItem.Add(GSI1, GSI1AttributeValue(item.ProjectId));

            dbItem.Add("UserId", StringAttributeValue(item.UserId));
            dbItem.Add("ProjectId", StringAttributeValue(item.ProjectId));
            dbItem.Add("Role", StringAttributeValue(item.Role));
            return dbItem;
        }

        protected override UserProject FromDynamoDb(Dictionary<string, AttributeValue> item)
        {
            var result = new UserProject();
            result.UserId = GetStringAttributeValue("UserId", item);
            result.ProjectId = GetStringAttributeValue("ProjectId", item);
            result.Role = GetStringAttributeValue("Role", item);
            return result;
        }



        public async Task<IList<UserProject>> GetUserProjectByProjectAsync(string projectId)
        {
            return await GetGSI1ItemsByParentIdAsync(projectId);
        }

        public async Task DeleteProjectFromUserAsync(string userId, string projectId)
        {
            await DeleteItemAsync(userId, userId+projectId);
        }

    }
}