# Usage - Independent entities

Independent entities are those not related to any other entity or if the entity is the ```one``` part in a ```one to many``` relationship. 

Define your entity as a POCO, no need to implement any interface or annotate anything.

```cs
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
```

Inherit from ```IndependentEntityRepository<TKey, TEntity>``` abstract class. This class provides methods intended to be used by independent entities and this is reflected in the way the PK and SK value are generated.

```cs
    public class UserRepository : IndependentEntityRepository<string, User>
    {
    }
```

Define partition key prefix and sort key prefix. This is the value that will be used when generating the values for PK, SK and GSI1. 

Set PKPrefix to a value that identifies the entity within the data model i.e ```"USER"```.

Set SKPrefix to a value that clearly states it's holding entity data and not a relationship i.e ```"METADATA"```.

```cs
    public UserRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
    {
        PKPrefix = "USER";
        SKPrefix = "METADATA";
    }
```

Internally, the ```GSI1``` attribute will be set to the value of PKPrefix property so when the GSI1 Index is queried, these items are found.

This will enable method ```GSI1QueryAllAsync()``` to retrieve the correct information. 

The typical implementation of the abstract methods will focus only on the data attributes to be mapped to and from DynamoDB.

```cs
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
```

```cs
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
```

Optionally (and recommended), define your own interface so it exposes methods with the relevant parameter and return values, it's also good practice to separate interface from implementation.

```cs
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
```

Make the repository class to implement the interface.

```cs
    public class UserRepository : IndependentEntityRepository<string, User>, IUserRepository
    {
    }
```

And add code to each method, it should be simple, most of the effort is to adapt to the custom interface.

```cs
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

```
