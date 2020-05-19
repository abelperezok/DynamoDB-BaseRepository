using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public interface IUserProjectRepository
    {
        Task AddProjectToUser(UserProject userProject);

        Task RemoveProjetFromUser(string userId, string projectId);

        Task<IList<UserProject>> GetProjectsByUserAsync(string userId);

        Task<IList<UserProject>> GetUsersByProjectAsync(string projectId);
    }


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

        // protected override string GetEntityKey(UserProject item)
        // {
        //     return item.UserId;
        // }

        // protected override string GetEntitySortKey(UserProject item)
        // {
        //     return item.UserId + item.ProjectId;
        // }

        // protected override Dictionary<string, AttributeValue> ToDynamoDb(UserProject item)
        // {
        //     var dbItem =  new Dictionary<string, AttributeValue>();            

        //     dbItem.Add(GSI1, GSI1AttributeValue(item.ProjectId));

        //     dbItem.Add("UserId", StringAttributeValue(item.UserId));
        //     dbItem.Add("ProjectId", StringAttributeValue(item.ProjectId));
        //     dbItem.Add("Role", StringAttributeValue(item.Role));
        //     return dbItem;
        // }

        // protected override UserProject FromDynamoDb(Dictionary<string, AttributeValue> item)
        // {
        //     var result = new UserProject();
        //     result.UserId = GetStringAttributeValue("UserId", item);
        //     result.ProjectId = GetStringAttributeValue("ProjectId", item);
        //     result.Role = GetStringAttributeValue("Role", item);
        //     return result;
        // }



        // public async Task<IList<UserProject>> GetUserProjectByProjectAsync(string projectId)
        // {
        //     return await GetGSI1ItemsByParentIdAsync(projectId);
        // }

        // public async Task DeleteProjectFromUserAsync(string userId, string projectId)
        // {
        //     await DeleteItemAsync(userId, userId+projectId);
        // }

    }
}