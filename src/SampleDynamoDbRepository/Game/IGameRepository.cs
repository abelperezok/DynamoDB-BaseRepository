using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDynamoDbRepository
{
    public interface IGameRepository
    {
        Task AddGame(string userId, Game game);
        Task DeleteGame(string userId, string gameId);
        Task UpdateGame(string userId, Game game);
        Task<IList<Game>> GetGameList(string userId);
        Task<Game> GetGame(string userId, string gameId);
    }
}