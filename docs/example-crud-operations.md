# Example: CRUD operations

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

        Console.WriteLine("Listing all users");
        var users = await repo.GetAllItemsAsync();
        foreach (var item in users)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }

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
