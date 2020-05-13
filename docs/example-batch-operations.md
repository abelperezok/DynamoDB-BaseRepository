# Example: Batch operations

Batch operations are generally more efficient than then one by one counterpart. Here are some examples illustrating how to use ```BatchAddItemAsync``` and ```BatchDeleteItemsAsync```.

## Example of batch insert.

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

## Example of batch delete.

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
