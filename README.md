# DynamoDB base repository

C# .NET Core implementation of the repository pattern using DynamoDB as data store using hierarchical data modelling strategy overloading the partition and sort key as well secondary index.

This implementation aims to solve the most common data persistence use cases ranging from single entities to more complex data models.

Key features:
* Pre-packaged CRUD and batch operations.
* Generic design for flexibility of data types.
* Extensible allowing the addition of specific functionality.


## Content

[Quick usage guide](#quick-usage-guide)

[Data model assumptions](docs/data-model-assumptions.md)

[Usage - Single entities](docs/usage-single-entities.md)

[Methods reference](docs/methods-reference.md)

[Example: CRUD operations](docs/example-crud-operations.md)

[Example: Batch operations](docs/example-batch-operations.md)

## Quick usage guide

1. Define your entity as a POCO, no interface or annotations required.

```cs
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
```

2. Inherit from ```DynamoDbRepository<TKey, TEntity>``` abstract class.

```cs
    public class UserRepository : DynamoDbRepository<string, User>
    {
    }
```

3. Define partition key prefix and sort key prefix in constructor.
 
```cs
    public UserRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
    {
        PKPrefix = "USER";
        SKPrefix = "METADATA";
    }
```

4. Override ```TKey GetEntityKey(TEntity item)``` abstract method to return the entity identifier.

```cs
    protected override string GetEntityKey(User item)
    {
        return item.Id;
    }
```

5. Override ```Dictionary<string, AttributeValue> ToDynamoDb(TEntity item)``` abstract method to map the entity object to a DynamoDB attribute dictionary.

```cs
    protected override Dictionary<string, AttributeValue> ToDynamoDb(User item)
    {
        var dbItem = new Dictionary<string, AttributeValue>();        
        dbItem.Add("Id", StringAttributeValue(item.Id));
        dbItem.Add("Name", StringAttributeValue(item.Name));
        return dbItem;
    }
```

1. Override ```TEntity FromDynamoDb(Dictionary<string, AttributeValue> item)``` abstract method to map the DynamoDB attribute dictionary to an entity object.

```cs
    protected override User FromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        var result = new User();
        result.Id = GetStringAttributeValue("Id", item);
        result.Name = GetStringAttributeValue("Name", item);
        return result;
    }
```






