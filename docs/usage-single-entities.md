# Usage - single entities

To manage entities with no relationship with any other




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

Inherit from ```DynamoDbRepository<TKey, TEntity>``` abstract class.

Define partition key prefix and sort key prefix. This is the value that will be used when generating the values for PK, SK and GSI1 in conjunction with the Id supplied as part of the entity to persist.

These values can be arbitrary values provided PKPrefix is unique across all entities that will be stored in that table.

```cs
    public UserRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
    {
        PKPrefix = "USER";
        SKPrefix = "METADATA";
    }
```

Override **GetEntityKey** abstract method, this method should return the Identity of the given entity. Normally it will return the value of the Id property.

```cs
    protected override string GetEntityKey(User item)
    {
        return item.Id;
    }
```

Override **ToDynamoDb** abstract method, this method should transform an instance of the entity into a DynamoDB attribute dictionary. The calling methods  deal with the PK, SK and GSI attributes. So your only responsibility is to map the actual data attributes. 

Some helper methods are provided to reduce the boilerplate code:

* StringAttributeValue converts a **string** to an AttributeValue expected by DynamoDB.
* NumberAttributeValue (**int** and **double**) converts a numeric value to an AttributeValue expected by DynamoDB.

Example of typical ```ToDynamoDb``` implementation:

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

Override **FromDynamoDb** abstract method. This method should transform an instance of a DynamoDB attribute dictionary into an instance of the entity.

Some helper methods are provided to reduce the boilerplate code:

* GetStringAttributeValue converts an AttributeValue from DynamoDB to a string.
* GetNumberInt32AttributeValue converts an AttributeValue from DynamoDB to an int.
* GetNumberDoubleAttributeValue converts an AttributeValue from DynamoDB to a double.

Example of typical ```FromDynamoDb``` implementation:

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
