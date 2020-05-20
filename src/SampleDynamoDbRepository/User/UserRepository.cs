using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public class UserRepository : IndependentEntityRepository<string, User>, IUserRepository
    {
        public UserRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "METADATA";
        }

        protected override DynamoDBItem ToDynamoDb(User item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddString("Id", item.Id);
            dbItem.AddString("Name", item.Name);
            dbItem.AddString("FirstName", item.FirstName);
            dbItem.AddString("LastName", item.LastName);
            dbItem.AddString("Email", item.Email);
            return dbItem;
        }

        protected override User FromDynamoDb(DynamoDBItem item)
        {
            var result = new User();
            result.Id = item.GetString("Id");
            result.Name = item.GetString("Name");
            result.FirstName = item.GetString("FirstName");
            result.LastName = item.GetString("LastName");
            result.Email = item.GetString("Email");
            return result;
        }

        public async Task AddUser(User user)
        {
            await AddItemAsync(user.Id, user);
        }

        public async Task DeleteUser(string userId)
        {
            await DeleteItemAsync(userId);
        }

        public async Task<IList<User>> GetUserList()
        {
            return await GSI1QueryAllAsync();
        }

        public async Task UpdateUser(User user)
        {
            await AddItemAsync(user.Id, user);
        }

        public async Task<User> GetUser(string userId)
        {
            return await GetItemAsync(userId);
        }

        public async Task BatchAddUsers(IEnumerable<User> items)
        {
            await BatchAddItemsAsync(items.Select(x => new KeyValuePair<string, User>(x.Id, x)));
        }

        public async Task BatchDeleteUsers(IEnumerable<User> items)
        {
            await BatchDeleteItemsAsync(items.Select(x => x.Id));
        }
    }
}