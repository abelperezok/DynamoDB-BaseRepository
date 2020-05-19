// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Amazon.DynamoDBv2.Model;
// using DynamoDbRepository;

// namespace SampleDynamoDbRepository
// {
//     public class GameRepository : DynamoDbRepository<string, Game>
//     {
//         public GameRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
//         {
//             PKPrefix = "USER";
//             SKPrefix = "GAME";
//         }

//         protected override string GetEntityKey(Game item)
//         {
//             return item.Id;
//         }

//         protected override Dictionary<string, AttributeValue> ToDynamoDb(Game item)
//         {
//             var dbItem = new Dictionary<string, AttributeValue>();
//             dbItem.Add("Id", StringAttributeValue(item.Id));
//             dbItem.Add("Name", StringAttributeValue(item.Name));
//             return dbItem;
//         }

//         protected override Game FromDynamoDb(Dictionary<string, AttributeValue> item)
//         {
//             var result = new Game();
//             result.Id = GetStringAttributeValue("Id", item);
//             result.Name = GetStringAttributeValue("Name", item);
//             return result;
//         }

//         public async Task<IList<Game>> GetGamesByUserAsync(string userId)
//         {
//             return await GetTableItemsByParentIdAsync(userId);
//         }

//         public async Task AddGameForUserAsync(string userId, Game item)
//         {
//             await AddItemAsync(userId, item);
//         }

//         public async Task DeleteGameFromUserAsync(string userId, string gameId)
//         {
//             await DeleteItemAsync(userId, gameId);
//         }
//     }
// }