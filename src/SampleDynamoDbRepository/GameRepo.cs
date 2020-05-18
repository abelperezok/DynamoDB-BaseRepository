using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
    public interface IGameRepo
    {
        Task AddGame(string userId, Game game);
        Task DeleteGame(string userId, string gameId);
        Task UpdateGame(string userId, Game game);
        Task<IList<Game>> GetGameList(string userId);
        Task<Game> GetGame(string userId, string gameId);
    }

    public class GameRepo : DependentEntityRepository<string, Game>, IGameRepo
    {
        public GameRepo(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "GAME";
        }

        protected override DynamoDBItem ToDynamoDb(Game item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddStringValue("Id", item.Id);
            dbItem.AddStringValue("Name", item.Name);
            return dbItem;
        }

        protected override Game FromDynamoDb(DynamoDBItem item)
        {
            var result = new Game();
            result.Id = item.GetStringValue("Id");
            result.Name = item.GetStringValue("Name");
            return result;
        }





        public Task AddGame(string userId, Game game)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteGame(string userId, string gameId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Game> GetGame(string userId, string gameId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Game>> GetGameList(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateGame(string userId, Game game)
        {
            throw new System.NotImplementedException();
        }


    }
}