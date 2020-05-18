using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public interface IUserRepo
    {
        Task AddUser(User user);
        Task DeleteUser(string userId);
        Task UpdateUser(User user);
        Task<IList<User>> GetUserList();
        Task<User> GetUser(string userId);
    }

    public class UserRepo : IndependentEntityRepository<string, User>, IUserRepo
    {
        public UserRepo(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "METADATA";
        }

        protected override DynamoDBItem ToDynamoDb(User item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddStringValue("Id", item.Id);
            dbItem.AddStringValue("Name", item.Name);
            dbItem.AddStringValue("FirstName", item.FirstName);
            dbItem.AddStringValue("LastName", item.LastName);
            dbItem.AddStringValue("Email", item.Email);
            return dbItem;
        }

        protected override User FromDynamoDb(DynamoDBItem item)
        {
            var result = new User();
            result.Id = item.GetStringValue("Id");
            result.Name = item.GetStringValue("Name");
            result.FirstName = item.GetStringValue("FirstName");
            result.LastName = item.GetStringValue("LastName");
            result.Email = item.GetStringValue("Email");
            return result;
        }


        public async Task AddUser(User user)
        {
            // var dbItem = ToDynamoDb(user);
            // await _dynamoDbClient.PutItemAsync(user.Id, user.Id, dbItem);
            await AddItemAsync(user.Id, user);
        }

        public async Task DeleteUser(string userId)
        {
            // await _dynamoDbClient.DeleteItemAsync(userId, userId);
            await DeleteItemAsync(userId);
        }

        public async Task<IList<User>> GetUserList()
        {
            // var queryRq =  _dynamoDbClient.GetGSI1AllQueryRequest(GSI1Prefix, SKPrefix);
            // var result = await _dynamoDbClient.QueryAsync(queryRq);
            // return result.Select(FromDynamoDb).ToList();

            return await GSI1QueryAllAsync();
        }

        public async Task UpdateUser(User user)
        {
            await AddItemAsync(user.Id, user);
            // var dbItem = ToDynamoDb(user);
            // await _dynamoDbClient.PutItemAsync(user.Id, user.Id, dbItem);
        }

        public async Task<User> GetUser(string userId)
        {
            return await GetItemAsync(userId);
            // var item = await _dynamoDbClient.GetItemAsync(userId, userId);
            // if (!item.IsEmpty)
            //     return FromDynamoDb(item);

            // return default(User);
        }
    }
}