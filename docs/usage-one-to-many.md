# Usage - One to many relationship

The ```one``` part in a ```one to many``` relationship is treated the same way as single entities with no relationship. This section explains the  ```many``` part in a ```one to many``` relationship. 

Define your entity as a POCO, it's entirely optional to include the parent entity identifier, in this example it's not included. It's left to the user's choice in accordance with the rest of the model.

In this example one User has many Games.

```cs
    public class Game
    {
        public string Id { get; set; }

        public string Name { get; set; }
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

To perform CRUD operations, it's required to supply the parent entity's identifier, for which there are separate version of the methods available. 

* ```Task<IList<TEntity>> GetItemsByParentIdAsync(TKey pkId)```
  
* ```Task AddItemAsync(TKey pkId, TEntity item)```

* ```Task UpdateItemAsync(TKey pkId, TEntity item)```

* ```Task DeleteItemAsync(TKey pkId, TKey skId)```


It's the user's choice whether to use the same methods or create more specific and better named ones. The implementation should be simple, only calling the corresponding base version. 

```cs

    public async Task<IList<Game>> GetGamesByUserAsync(string userId)
    {
        return await GetItemsByParentIdAsync(userId);
    }

    public async Task AddGameForUserAsync(string userId, Game item)
    {
        await AddItemAsync(userId, item);
    }

    public async Task DeleteGameFromUserAsync(string userId, string gameId)
    {
        await DeleteItemAsync(userId, gameId);
    }
```

The typical implementation of the abstract methods will focus only on the data attributes to be mapped to and from DynamoDB.

```cs
    protected override string GetEntityKey(Game item)
    {
        return item.Id;
    }

    protected override Dictionary<string, AttributeValue> ToDynamoDb(Game item)
    {
        var dbItem = new Dictionary<string, AttributeValue>();
        dbItem.Add("Id", StringAttributeValue(item.Id));
        dbItem.Add("Name", StringAttributeValue(item.Name));
        return dbItem;
    }

    protected override Game FromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        var result = new Game();
        result.Id = GetStringAttributeValue("Id", item);
        result.Name = GetStringAttributeValue("Name", item);
        return result;
    }
```