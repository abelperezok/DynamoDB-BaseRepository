using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class UserProjectRepository : AssociativeEntityRepository<string, UserProject>, IUserProjectRepository
    {

        public UserProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "USER_PROJECT";
            GSI1Prefix = "PROJECT";
        }
        protected override string GetRelationKey(string parent1Key, string parent2Key)
        {
            return parent1Key + parent2Key;
        }

        protected override DynamoDBItem ToDynamoDb(UserProject item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddString("UserId", item.UserId);
            dbItem.AddString("ProjectId", item.ProjectId);
            dbItem.AddString("Role", item.Role);
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


        public async Task AddProjectToUser(UserProject userProject)
        {
            await AddItemAsync(userProject.UserId, userProject.ProjectId, userProject);
        }

        public async Task RemoveProjectFromUser(string userId, string projectId)
        {
            await DeleteItemAsync(userId, projectId);
        }

        public async Task<IList<UserProject>> GetProjectsByUserAsync(string userId)
        {
            return await TableQueryItemsByParentIdAsync(userId);
        }

        public async Task<IList<UserProject>> GetUsersByProjectAsync(string projectId)
        {
            return await GSI1QueryItemsByParentIdAsync(projectId);
        }

        public async Task BatchAddProjectsToUser(string userId, IEnumerable<UserProject> userProjects)
        {
            await BatchAddItemsAsync(userId, userProjects.Select(x => new KeyValuePair<string, UserProject>(x.ProjectId, x)));
        }

        public async Task BatchRemoveProjectsFromUser(string userId, IEnumerable<UserProject> userProjects)
        {
            await BatchDeleteItemsAsync(userId, userProjects.Select(x => x.ProjectId));
        }
    }
}