# DynamoDB base repository

[![.NET Core Github Actions badge](https://github.com/abelperezok/DynamoDB-BaseRepository/workflows/.NET%20Core/badge.svg)](https://github.com/abelperezok/DynamoDB-BaseRepository/actions)
[![Coverage Status](https://coveralls.io/repos/github/abelperezok/DynamoDB-BaseRepository/badge.svg?branch=master)](https://coveralls.io/github/abelperezok/DynamoDB-BaseRepository?branch=master)

C# .NET Core implementation of the repository pattern using DynamoDB as data store using single table and hierarchical data modelling approach overloading the partition and sort key as well secondary index.

This implementation aims to solve the most common data persistence use cases ranging from independent entities to more complex data models.

Key features:
* Ready to use CRUD operations.
* Ready to use batch operations.
* Generic design for flexibility of data types.
* One to many relationship.
* Many to many relationships


## Content

[Quick usage guide](#quick-usage-guide)

[Data model assumptions](docs/data-model-assumptions.md)

[Usage - Independent entities](docs/usage-independent-entities.md)

[Usage - One to many](docs/usage-one-to-many.md)

[Usage - Many to many](docs/usage-many-to-many.md)

[Example: CRUD operations](docs/example-crud-operations.md)

[Example: Batch operations](docs/example-batch-operations.md)

## Quick usage guide

1. Define your entity as a POCO, no interface or annotations required.

```cs
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }
    }
```

2. Inherit from ```SimpleRepository<TKey, TEntity>``` abstract class.

```cs
    public class PersonRepository : SimpleRepository<int, Person>
    {
    }
```

3. Define partition key prefix and sort key prefix in constructor.
 
```cs
    public PersonRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
    {
        PKPrefix = "PERSON";
        SKPrefix = "METADATA";
    }
```

4. Override ```TKey GetEntityKey(TEntity item)``` abstract method to return the entity identifier.

```cs
    protected override int GetEntityKey(Person item)
    {
        return item.Id;
    }
```

5. Override ```DynamoDBItem ToDynamoDb(TEntity item)``` abstract method to map the entity object to a DynamoDB attribute dictionary.

```cs
     protected override DynamoDBItem ToDynamoDb(Person item)
    {
        var dbItem = new DynamoDBItem();
        dbItem.AddNumber("Id", item.Id);
        dbItem.AddString("Name", item.Name);
        dbItem.AddString("Email", item.Email);
        dbItem.AddNumber("Age", item.Age);
        return dbItem;
    }
```

6. Override ```TEntity FromDynamoDb(DynamoDBItem item)``` abstract method to map the DynamoDB attribute dictionary to an entity object.

```cs
    protected override Person FromDynamoDb(DynamoDBItem item)
    {
        var result = new Person();
        result.Id = item.GetInt32("Id");
        result.Name = item.GetString("Name");
        result.Email = item.GetString("Email");
        result.Age = item.GetInt32("Age");
        return result;
    }
```

7. Use the available methods either directly or through the interface.

```cs
    public static async Task TestCRUD_PersonRepository()
    {
        // Create a new PersonRepository
        ISimpleRepository<int, Person> repo = new PersonRepository(_tableName);

        // Prepare a Person instance
        var p1 = new Person
        {
            Id = 1,
            Name = "personA",
            Email = "pa@test.com",
            Age = 35
        };

        Console.WriteLine("* Adding Person 1");
        // Add a new person
        await repo.Add(p1);

        Console.WriteLine("* Getting the list");
        // Get the full list
        var list = await repo.GetList();
        foreach (var item in list)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }

        Console.ReadKey();

        Console.WriteLine("* Getting Person 1");
        // Get an individual Person by its Id
        var found1 = await repo.Get(p1.Id);
        Console.WriteLine(JsonSerializer.Serialize(found1));

        Console.WriteLine("* Deleting Person 1");
        // Delete an individual Person by its Id
        await repo.Delete(p1.Id);
    }
```
