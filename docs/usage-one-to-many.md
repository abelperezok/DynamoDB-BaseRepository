# Usage - Item collection - One to many relationship

The ```one``` part in a ```one to many``` relationship is treated the same way as single entities with no relationship. This section explains the  ```many``` part in a ```one to many``` relationship. 

Define your entity as a POCO, it's entirely optional to include the parent entity identifier, in this example it's not included. It's left to the user's choice in accordance with the rest of the model.

> **Note** - In this example one **User** has many **Games**.

```cs
    public class Game
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
```

Inherit from ```DependentEntityRepository<TKey, TEntity>``` abstract class. This class provides methods intended to be used by dependent entities which require the parent entity identifier and this is reflected in the way the PK and SK value are generated.

```cs
    public class GameRepository : DependentEntityRepository<string, Game>
    {
    }
```

Define partition key prefix and sort key prefix. This is the value that will be used when generating the values for PK, SK and GSI1. 

Set PKPrefix to a value that identifies the **parent entity** within the data model i.e ```"USER"```. 

Set SKPrefix to a value that identifies the **entity in question** within the data model i.e ```"GAME"```.

```cs
    public GameRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
    {
        PKPrefix = "USER";
        SKPrefix = "GAME";
    }
```

Internally, the ```GSI1``` attribute will be left empty so when the GSI1 Index is queried, these items are not found or confused with another entity.

All this will enable the method ```TableQueryItemsByParentIdAsync(TKey)``` to retrieve the correct information. 

The typical implementation of the abstract methods will focus only on the data attributes to be mapped to and from DynamoDB.

```cs
    protected override DynamoDBItem ToDynamoDb(Game item)
    {
        var dbItem = new DynamoDBItem();
        dbItem.AddString("Id", item.Id);
        dbItem.AddString("Name", item.Name);
        return dbItem;
    }
```

```cs
    protected override Game FromDynamoDb(DynamoDBItem item)
    {
        var result = new Game();
        result.Id = item.GetString("Id");
        result.Name = item.GetString("Name");
        return result;
    }
```

Optionally (and recommended), define your own interface so it exposes methods with the relevant parameter and return values, it's also good practice to separate interface from implementation.

```cs
    public interface IGameRepository
    {
        Task AddGame(string userId, Game game);

        Task DeleteGame(string userId, string gameId);

        Task UpdateGame(string userId, Game game);

        Task<IList<Game>> GetGameList(string userId);

        Task<Game> GetGame(string userId, string gameId);

        Task BatchAddGames(string userId, IEnumerable<Game> items);

        Task BatchDeleteGames(string userId, IEnumerable<Game> items);
    }
```

Make the repository class to implement the interface.

```cs
    public class GameRepository : DependentEntityRepository<string, Game>, IGameRepository
    {
    }
```

And add code to each method, it should be simple, most of the effort is to adapt to the custom interface.

```cs
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

    public async Task BatchAddGames(string userId, IEnumerable<Game> items)
    {
        await BatchAddItemsAsync(userId, items.Select(x => new KeyValuePair<string, Game>(x.Id, x)));
    }

    public async Task BatchDeleteGames(string userId, IEnumerable<Game> items)
    {
        await BatchDeleteItemsAsync(userId, items.Select(x => x.Id));
    }

```
