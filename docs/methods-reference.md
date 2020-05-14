# Methods reference

```DynamoDbRepository<TKey, TEntity>``` abstract class defines the base functionalities to perform create, retrieve, update and delete operations. There are three methods that need to be overridden by inheritors.

## Abstract methods

* ```TKey GetEntityKey(TEntity item)``` This method should return the Identity of the given entity. Normally it will return the value of the ```Id``` property or the equivalent in your model.

    Example of typical ```GetEntityKey``` implementation:
    ```cs
    protected override string GetEntityKey(User item)
    {
        return item.Id;
    }
    ```

* ```Dictionary<string, AttributeValue> ToDynamoDb(TEntity item)```  This method should transform an instance of the entity into a DynamoDB attribute dictionary. The calling methods will deal with the PK, SK and GSI attributes. So your only responsibility is to map the actual data attributes.

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

* ```TEntity FromDynamoDb(Dictionary<string, AttributeValue> item)``` This method should transform an instance of a DynamoDB attribute dictionary into an instance of the entity.

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

## Operations methods

The base class provides basic CRUD like methods already implemented  encapsulating all the low level DynamoDB requests / response handling. 

The version with an additional parameter ```TKey pkId``` which is intended for the ```many``` part in a ```one to many``` relationship.

The version without ```TKey pkId``` parameter is intended for single entities or the ```one``` part in a ```one to many``` relationship.

### Single item retrieving operations:

* ```Task<TEntity> GetItemAsync(TKey id)``` 

* ```Task<TEntity> GetItemAsync(TKey pkId, TKey skId)```

### Multiple items retrieving operations:

* ```Task<IList<TEntity>> GetAllItemsAsync()```

* ```Task<IList<TEntity>> GetItemsByParentIdAsync(TKey pkId)```

* ```Task<int> CountAsync()```

### Single item add, update, delete:

* ```Task AddItemAsync(TEntity item)```
* ```Task AddItemAsync(TKey pkId, TEntity item)```
  
* ```Task UpdateItemAsync(TEntity item)```
* ```Task UpdateItemAsync(TKey pkId, TEntity item)```

* ```Task DeleteItemAsync(TKey id)```
* ```Task DeleteItemAsync(TKey pkId, TKey skId)```

### Batch insert and delete: 

* ```Task BatchAddItemAsync(IEnumerable<TEntity> items)```
* ```Task BatchAddItemAsync(TKey pkId, IEnumerable<TEntity> items)```

* ```Task BatchDeleteItemsAsync(IEnumerable<TEntity> items)```
* ```Task BatchDeleteItemsAsync(TKey pkId, IEnumerable<TEntity> items)```