using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDynamoDbRepository
{
    public interface IProjectRepository
    {
        Task AddProject(Project project);
        Task DeleteProject(string projectId);
        Task UpdateProject(Project project);
        Task<IList<Project>> GetProjectList();
        Task<Project> GetProject(string projectId);
    }
}