# Usage - Single entities

Single entities are those with no relationship with any other entity or if the entity is the ```one``` part in a ```one to many``` relationship. 

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

This will enable methods ```GetAllItemsAsync()``` and ```CountAsync()``` to retrieve the correct information. 

The typical implementation of the abstract methods will focus only on the data attributes to be mapped to and from DynamoDB.

```cs
    protected override string GetEntityKey(User item)
    {
        return item.Id;
    }
```

```cs
    protected override Dictionary<string, AttributeValue> ToDynamoDb(User item)
    {
        var dbItem = new Dictionary<string, AttributeValue>();        
        dbItem.Add("Id", StringAttributeValue(item.Id));
        dbItem.Add("Name", StringAttributeValue(item.Name));
        dbItem.Add("FirstName", StringAttributeValue(item.FirstName));
        dbItem.Add("LastName", StringAttributeValue(item.LastName));
        dbItem.Add("Email", StringAttributeValue(item.Email));
        return dbItem;
    }
```

```cs
    protected override User FromDynamoDb(Dictionary<string, AttributeValue> item)
    {
        var result = new User();
        result.Id = GetStringAttributeValue("Id", item);
        result.Name = GetStringAttributeValue("Name", item);
        result.FirstName = GetStringAttributeValue("FirstName", item);
        result.LastName = GetStringAttributeValue("LastName", item);
        result.Email = GetStringAttributeValue("Email", item);
        return result;
    }
```