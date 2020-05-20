using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDynamoDbRepository
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task DeleteUser(string userId);
        Task UpdateUser(User user);
        Task<IList<User>> GetUserList();
        Task<User> GetUser(string userId);
        Task BatchAddUsers(IEnumerable<User> items);
        Task BatchDeleteUsers(IEnumerable<User> items);
    }
}