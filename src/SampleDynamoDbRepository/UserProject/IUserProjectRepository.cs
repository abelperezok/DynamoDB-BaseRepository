using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDynamoDbRepository
{
    public interface IUserProjectRepository
    {
        Task AddProjectToUser(UserProject userProject);

        Task RemoveProjetFromUser(string userId, string projectId);

        Task<IList<UserProject>> GetProjectsByUserAsync(string userId);

        Task<IList<UserProject>> GetUsersByProjectAsync(string projectId);
    }
}