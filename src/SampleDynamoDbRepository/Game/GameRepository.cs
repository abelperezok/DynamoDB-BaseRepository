using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDbRepository;

namespace SampleDynamoDbRepository
{

    public class GameRepository : DependentEntityRepository<string, Game>, IGameRepository
    {
        public GameRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "GAME";
        }

        protected override DynamoDBItem ToDynamoDb(Game item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddString("Id", item.Id);
            dbItem.AddString("Name", item.Name);
            return dbItem;
        }

        protected override Game FromDynamoDb(DynamoDBItem item)
        {
            var result = new Game();
            result.Id = item.GetString("Id");
            result.Name = item.GetString("Name");
            return result;
        }

        public async Task AddGame(string userId, Game game)
        {
            await AddItemAsync(userId, game.Id, game);
        }

        public async Task DeleteGame(string userId, string gameId)
        {
            await DeleteItemAsync(userId, gameId);
        }

        public async Task<Game> GetGame(string userId, string gameId)
        {
            return await GetItemAsync(userId, gameId);
        }

        public async Task<IList<Game>> GetGameList(string userId)
        {
            return await TableQueryItemsByParentIdAsync(userId);
        }

        public async Task UpdateGame(string userId, Game game)
        {
            await AddItemAsync(userId, game.Id, game);
        }
    }
}