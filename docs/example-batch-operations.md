# Example: Batch operations

Batch operations are generally more efficient than then one by one counterpart. Here are some examples illustrating how to use them.

## Example of batch insert for an independent entity

```cs
    private static async Task TestUserRepositoryBatchAddItems()
    {
        IUserRepository repo = new UserRepository(_tableName);
        var itemsToCreate = new List<User>();
        for (int i = 50; i < 60; i++)
        {
            var uA = new User { Id = "A" + i, Name = "userA" + i, FirstName = "User" + i, LastName = "A" + i, Email = $"a{i}@test.com" };
            itemsToCreate.Add(uA);
            Console.WriteLine("* Adding to list user " + uA.Id);

        }
        await repo.BatchAddUsers(itemsToCreate);
        Console.WriteLine("***** Done adding all users");

        Console.WriteLine("* Getting all users");
        var users = await repo.GetUserList();
        foreach (var item in users)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }
    }
```

## Example of batch insert for a dependent entity

```cs
    private static async Task TestGameRepositoryBatchAddItems()
    {
        var user = new User { Id = "U1" };

        IGameRepository repo = new GameRepository(_tableName);
        var itemsToCreate = new List<Game>();
        for (int i = 50; i < 60; i++)
        {
            var g = new Game { Id = "G" + i, Name = "Game G" + i };
            itemsToCreate.Add(g);
            Console.WriteLine("* Adding to list game " + g.Id);

        }
        await repo.BatchAddGames(user.Id, itemsToCreate);
        Console.WriteLine("***** Done adding all games");

        Console.WriteLine("* Getting all games");
        var items = await repo.GetGameList(user.Id);
        foreach (var item in items)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }
    }
```

## Example of batch delete for an independent entity

```cs
    private static async Task TestUserRepositoryBatchDeleteItems()
    {
        IUserRepository repo = new UserRepository(_tableName);
        var itemsToDelete = new List<User>();
        for (int i = 50; i < 60; i++)
        {
            var uA = new User { Id = "A" + i };
            itemsToDelete.Add(uA);
            Console.WriteLine("* Adding to delete list, user " + uA.Id);

        }
        await repo.BatchDeleteUsers(itemsToDelete);
        Console.WriteLine("***** Done deleting all users");

        Console.WriteLine("* Getting all users");
        var users = await repo.GetUserList();
        foreach (var item in users)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }
    }
```

## Example of batch delete for a dependent entity

```cs
    private static async Task TestGameRepositoryBatchDeleteItems()
    {
        var user = new User { Id = "U1" };

        IGameRepository repo = new GameRepository(_tableName);
        var itemsToDelete = new List<Game>();
        for (int i = 50; i < 60; i++)
        {
            var g = new Game { Id = "G" + i };
            itemsToDelete.Add(g);
            Console.WriteLine("* Adding to delete list, game " + g.Id);

        }
        await repo.BatchDeleteGames(user.Id, itemsToDelete);
        Console.WriteLine("***** Done deleting all games");

        Console.WriteLine("* Getting all games");
        var games = await repo.GetGameList(user.Id);
        foreach (var item in games)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }
    }
```
