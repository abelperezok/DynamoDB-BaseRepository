# Example: CRUD operations

Here is an example of performing all basic CRUD operations in a sequence for an independent entity (User).

```cs
    private static async Task TestCRUD_UserRepository()
    {
        IUserRepository repo = new UserRepository(_tableName);

        var u1 = new User { Id = "U1", Name = "userU1", FirstName = "User", LastName = "U1", Email = "u1@test.com" };
        Console.WriteLine("* Creating user U1");
        await repo.AddUser(u1);

        var u2 = new User { Id = "U2", Name = "userU2", FirstName = "User", LastName = "U2", Email = "u2@test.com" };
        Console.WriteLine("* Creating user U2");
        await repo.AddUser(u2);

        Console.WriteLine("* Getting all users");
        var users = await repo.GetUserList();
        foreach (var item in users)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }

        Console.WriteLine("* Getting user U1");
        var found1 = await repo.GetUser(u1.Id);
        Console.WriteLine(JsonSerializer.Serialize(found1));

        Console.WriteLine("* Getting user U2");
        var found2 = await repo.GetUser(u2.Id);
        Console.WriteLine(JsonSerializer.Serialize(found2));

        Console.WriteLine("* Deleting user U1");
        await repo.DeleteUser(u1.Id);

        Console.WriteLine("* Deleting user U2");
        await repo.DeleteUser(u2.Id);
    }
```

Here is an example of performing all basic CRUD operations in a sequence for a dependent entity (Game) using generic methods (not interface).

```cs
    private static async Task TestCRUD_GameRepository()
    {
        var repo = new GameRepository(_tableName);
        var g1 = new Game { Id = "G1", Name = "Game G1" };
        Console.WriteLine("* Creating game G1");
        await repo.AddItemAsync("U1", "G1", g1);

        var g2 = new Game { Id = "G2", Name = "Game G2" };
        Console.WriteLine("* Creating game G2");
        await repo.AddItemAsync("U1", "G2", g2);

        Console.WriteLine("* Getting all users");
        var games = await repo.TableQueryItemsByParentIdAsync("U1");
        foreach (var item in games)
        {
            Console.WriteLine(JsonSerializer.Serialize(item));
        }

        Console.WriteLine("* Getting game G1");
        var found1 = await repo.GetItemAsync("U1", g1.Id);
        Console.WriteLine(JsonSerializer.Serialize(found1));

        Console.WriteLine("* Getting game G2");
        var found2 = await repo.GetItemAsync("U1", g2.Id);
        Console.WriteLine(JsonSerializer.Serialize(found2));

        Console.WriteLine("* Deleting game G1");
        await repo.DeleteItemAsync("U1", g1.Id);

        Console.WriteLine("* Deleting game G2");
        await repo.DeleteItemAsync("U1", g2.Id);
    }
```