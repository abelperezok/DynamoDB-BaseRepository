using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDynamoDbRepository
{
    public interface IUserProjectRepository
    {
        Task AddProjectToUser(UserProject userProject);

        Task RemoveProjectFromUser(string userId, string projectId);

        Task<IList<UserProject>> GetProjectsByUserAsync(string userId);

        Task<IList<UserProject>> GetUsersByProjectAsync(string projectId);

        Task BatchAddProjectsToUser(string userId, IEnumerable<UserProject> userProjects);

        Task BatchRemoveProjectsFromUser(string userId, IEnumerable<UserProject> userProjects);
    }
}