# Usage - Item collection - Many to many relationship

This section explains how to handle a typical ```many to many``` relationship following the same pattern. 

Define your relation as a POCO, where normally it should it include both parent entities identifier. Depending on the nature of the model, you can also add other attributes.

> **Note** - In this example one **User** participates in many **Projects** and one **Project** can have many **Users**. A **User** in a **Project** can be either owner or member.

```cs
    public class UserProject
    {
        public string UserId { get; set; }
        public string ProjectId { get; set; }        
        public string Role { get; set; }
    }
```

Inherit from ```AssociativeEntityRepository<TKey, TEntity>``` abstract class. This class provides methods intended to be used by dependent entities which require both parent entities identifiers and this is reflected in the way the PK and SK value are generated.

```cs
    public class UserProjectRepository : AssociativeEntityRepository<string, UserProject>
    {
    }
```

Define partition key, sort key and GSI1 prefix. This is the value that will be used when generating the values for PK, SK and GSI1. At this point, a choice has to be made regarding which of the parent entities is going to be PK and which is going to be GSI1. There is not right or wrong answer to this. It only determines the way queries are made to retrieve users by project or projects by user.

Set PKPrefix to a value that identifies the **"main" parent entity** within the data model i.e ```"USER"```. 

Set SKPrefix to a value that identifies the relationship within the data model i.e ```"USER_PROJECT"```.

Set GSI1Prefix to a value that identifies the **"second" entity in question** within the data model i.e ```"PROJECT"```.

```cs
    public UserProjectRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "USER";
            SKPrefix = "USER_PROJECT";
            GSI1Prefix = "PROJECT";
        }
```

All this will enable the query methods ```TableQueryItemsByParentIdAsync(TKey)``` and ```GSI1QueryItemsByParentIdAsync(TKey)``` to retrieve the correct information. In this case, query the table to retrieve projects by user and query the GSI to retrieve users by project.

There has to be a way to determine the unique key for the relationship, to solve this, override ```TKey GetRelationKey(TKey parent1Key, TKey parent2Key)``` abstract method. An easy way to make this key unique is just to concatenate both parents' keys. 

```cs
    protected override string GetRelationKey(string parent1Key, string parent2Key)
    {
        return parent1Key + parent2Key;
    }
```

The typical implementation of the other abstract methods will focus only on the data attributes to be mapped to and from DynamoDB.

```cs
    protected override DynamoDBItem ToDynamoDb(UserProject item)
    {
        var dbItem = new DynamoDBItem();
        dbItem.AddString("UserId", item.UserId);
        dbItem.AddString("ProjectId", item.ProjectId);
        dbItem.AddString("Role", item.Role);
        return dbItem;
    }
```

```cs
    protected override UserProject FromDynamoDb(DynamoDBItem item)
    {
        var result = new UserProject();
        result.UserId = item.GetString("UserId");
        result.ProjectId = item.GetString("ProjectId");
        result.Role = item.GetString("Role");
        return result;
    }
```

Optionally (and recommended), define your own interface so it exposes methods with the relevant parameter and return values, it's also good practice to separate interface from implementation.

```cs
    public interface IUserProjectRepository
    {
        Task AddProjectToUser(UserProject userProject);

        Task RemoveProjectFromUser(string userId, string projectId);

        Task<IList<UserProject>> GetProjectsByUserAsync(string userId);

        Task<IList<UserProject>> GetUsersByProjectAsync(string projectId);
    }
```

Make the repository class to implement the interface.

```cs
    public class UserProjectRepository : AssociativeEntityRepository<string, UserProject>, IUserProjectRepository
    {
    }
```

And add code to each method, it should be simple, most of the effort is to adapt to the custom interface.

```cs
    public async Task AddProjectToUser(UserProject userProject)
    {
        await AddItemAsync(userProject.UserId, userProject.ProjectId, userProject);
    }

    public async Task RemoveProjectFromUser(string userId, string projectId)
    {
        await DeleteItemAsync(userId, projectId);
    }

    public async Task<IList<UserProject>> GetProjectsByUserAsync(string userId)
    {
        return await TableQueryItemsByParentIdAsync(userId);
    }

    public async Task<IList<UserProject>> GetUsersByProjectAsync(string projectId)
    {
        return await GSI1QueryItemsByParentIdAsync(projectId);
    }
```
