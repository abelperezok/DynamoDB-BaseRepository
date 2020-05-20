using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class UserProjectRepository : DependentEntityRepository<string, UserProject>, IUserProjectRepository
    {

        public UserProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "USER_PROJECT";
            GSI1Prefix = "PROJECT";
        }

        protected override DynamoDBItem ToDynamoDb(UserProject item)
        {
            var dbItem = new DynamoDBItem();

            dbItem.AddString("UserId", item.UserId);
            dbItem.AddString("ProjectId", item.ProjectId);
            dbItem.AddString("Role", item.Role);

            dbItem.AddGSI1(GSI1Value(item.ProjectId));

            return dbItem;
        }

        protected override UserProject FromDynamoDb(DynamoDBItem item)
        {
            var result = new UserProject();
            result.UserId = item.GetString("UserId");
            result.ProjectId = item.GetString("ProjectId");
            result.Role = item.GetString("Role");
            return result;
        }


        private string GetRelationKey(string userId, string projectId)
        {
            return userId + projectId;
        }


        public async Task AddProjectToUser(UserProject userProject)
        {
            var relationKey = GetRelationKey(userProject.UserId, userProject.ProjectId);
            await AddItemAsync(userProject.UserId, relationKey, userProject);
        }

        public async Task RemoveProjetFromUser(string userId, string projectId)
        {
            var relationKey = GetRelationKey(userId, projectId);
            await DeleteItemAsync(userId, relationKey);
        }

        public async Task<IList<UserProject>> GetProjectsByUserAsync(string userId)
        {
            return await TableQueryItemsByParentIdAsync(userId);
        }

        public async Task<IList<UserProject>> GetUsersByProjectAsync(string projectId)
        {
            return await GSI1QueryItemsByParentIdAsync(projectId);
        }
    }
}