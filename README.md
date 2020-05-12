# DynamoDB base repository

C# .NET Core implementation of the repository pattern using DynamoDB as data store using hierarchical data.

## Content

[Data model assumptions](#data-model-assumptions)

[Usage](#usage)

[Example: CRUD operations](#example-crud-operations)

[Example: Batch operations](#example-batch-operations)


## Data model assumptions

When trying to generalize a concept, there has to be some assumptions. In this case, the table structure follows the general ideas about hierarchical data overloading the partition and sort keys as well as the GSI.

* Generic partition key "PK" string.
* Generic sort key "SK" string.
* Generic attribute "GSI1" string.
* GSI partition key is "GSI1".
* GSI sort key is also "SK".
* GSI projects all attributes.
* Other attributes such as ID, Name, Description, etc.


### Table with sample data

PK (S) | SK (S) | GSI1 | ID | Name | Description
-------|--------|------|----|------|------------
USER#U1 | METADATA#U1 | USER | U1 | Abel	
USER#U2 | METADATA#U2 | USER | U2 | Nestor	
PROJECT#P1 | METADATA#P1 | PROJECT | P1 | Project 1 | desc project 1
PROJECT#P2 | METADATA#P2 | PROJECT | P2 | Project 2 | desc project 2
PROJECT#P3 | METADATA#P3 | PROJECT | P3 | Project 3 | desc project 3
USER#U1  | GAME#G1 | GAME | G1 | Game 1 |
USER#U1  | GAME#G2 | GAME | G2 | Game 2 |

### GSI with sample data

PK (S) GSI1 | SK (S) SK | ID | Name | Description
------------|-----------|----|------|------------
USER | METADATA#U1 | U1 | Abel	  |
USER | METADATA#U2 | U2 | Nestor  |	
PROJECT | METADATA#P1 | P1 | Project 1 | desc project 1
PROJECT | METADATA#P2 | P2 | Project 2 | desc project 2
PROJECT | METADATA#P3 | P3 | Project 3 | desc project 3


### Queries

* Single item given the ID: Table PK = ENTITY#ID, SK = METADATA#ID
  * Get User U1: Table PK = USER#U1, SK = METADATA#U1
  * Get Project P1: Table PK = PROJECT#P1, SK = METADATA#P1
  * Get Game G1 for User U1: Table PK = USER#U1, SK = GAME#G1

* Multiple items per type: GSI PK = ENTITY
  * Get all users: GSI PK = USER
  * Get all projects: GSI PK = PROJECT

* Multiple items per parent instance: Table PK = PARENT_ENTITY#ID, SK = ENTITY#ID
  * Get all games by user U1 : Table PK = USER#U1, SK begins_with GAME

## Usage

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

The base class also provides basic CRUD like methods already implemented following this pattern. 

Retrieving operations:

* ```Task<TEntity> GetItemAsync(TKey id)``` 
* ```Task<IList<TEntity>> GetAllAsync()```
* ```Task<int> CountAsync()```

Single item add, update, delete:

* ```Task AddItemAsync(TEntity item)```
* ```Task UpdateItemAsync(TEntity item)```
* ```Task DeleteItemAsync(TKey id)```

Batch insert and delete: 
* ```Task BatchAddItemAsync(IEnumerable<TEntity> items)```
* ```Task BatchDeleteItemsAsync(IEnumerable<TEntity> items)```

## Example: CRUD operations

Here is an example of performing all basic CRUD operations in a sequence.

```cs
    private static async Task TestUserRepositoryCRUD()
    {
        var repo = new UserRepository(_tableName);

        var uA = new User { Id = "A", Name = "userA", FirstName = "User", LastName = "A", Email = "a@test.com" };
        Console.WriteLine("* Creating user A");
        await repo.AddItemAsync(uA);

        Console.WriteLine("* Retrieving user A");
        var uuA = await repo.GetItemAsync("A");
        if (uuA != null)
            Console.WriteLine(JsonSerializer.Serialize(uuA));
        else
            Console.WriteLine("not found");

        Console.ReadKey();


        uA.Name = "userAA";
        uA.Email = "aa-aa@test.com";
        uA.FirstName = "UserUser";
        uA.LastName = "AA AA";
        Console.WriteLine("* Updating user A - renamed to AA");
        await repo.UpdateItemAsync(uA);

        Console.WriteLine("* Retrieving user A after update");
        var uAUpdated = await repo.GetItemAsync("A");
        if (uAUpdated != null)
            Console.WriteLine(JsonSerializer.Serialize(uAUpdated));
        else
            Console.WriteLine("not found");

        Console.ReadKey();


        Console.WriteLine("* Deleting user A");
        await repo.DeleteItemAsync("A");

        Console.WriteLine("* Retrieving user A after deletion");
        var deletedA = await repo.GetItemAsync("A");
        if (deletedA != null)
            Console.WriteLine(JsonSerializer.Serialize(deletedA));
        else
            Console.WriteLine("not found");
    }
```

## Example: Batch operations

Here is an example of performing batch insert.

```cs
    private static async Task TestUserRepositoryBatchAddItems()
    {
        var repo = new UserRepository(_tableName);
        var itemsToCreate = new List<User>();
        for (int i = 50; i < 60; i++)
        {
            var uA = new User { Id = "A" + i, Name = "userA" + i, FirstName = "User" + i, LastName = "A" + i, Email = $"a{i}@test.com" };
            itemsToCreate.Add(uA);
            Console.WriteLine("* Adding to list user " + uA.Id);

        }
        await repo.BatchAddItemAsync(itemsToCreate);
        Console.WriteLine("***** Done adding all items");
    }
```

Here is an example of performing batch delete.

```cs
    private static async Task TestUserRepositoryBatchDeleteItems()
    {
        var repo = new UserRepository(_tableName);
        var itemsToDelete = new List<User>();
        for (int i = 50; i < 60; i++)
        {
            var uA = new User { Id = "A" + i };
            itemsToDelete.Add(uA);
            Console.WriteLine("* Adding to delete list, user " + uA.Id);

        }
        await repo.BatchDeleteItemsAsync(itemsToDelete);
        Console.WriteLine("***** Done deleting all items");
    }
```
