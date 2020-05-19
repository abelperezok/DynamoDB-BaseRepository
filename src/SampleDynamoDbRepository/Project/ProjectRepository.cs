using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class ProjectRepository : IndependentEntityRepository<string, Project>, IProjectRepository
    {
        public ProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "PROJECT";
            SKPrefix = "METADATA";
        }

        protected override DynamoDBItem ToDynamoDb(Project item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddString("Id", item.Id);
            dbItem.AddString("Name", item.Name);
            dbItem.AddString("Description", item.Description);
            return dbItem;
        }

        protected override Project FromDynamoDb(DynamoDBItem item)
        {
            var result = new Project();
            result.Id = item.GetString("Id");
            result.Name = item.GetString("Name");
            result.Description = item.GetString("Description");
            return result;
        }

        public async Task AddProject(Project project)
        {
            await AddItemAsync(project.Id, project);
        }

        public async Task DeleteProject(string projectId)
        {
            await DeleteItemAsync(projectId);
        }

        public async Task UpdateProject(Project project)
        {
            await AddItemAsync(project.Id, project);
        }

        public async Task<IList<Project>> GetProjectList()
        {
            return await GSI1QueryAllAsync();
        }

        public async Task<Project> GetProject(string projectId)
        {
            return await GetItemAsync(projectId);
        }
    }
}