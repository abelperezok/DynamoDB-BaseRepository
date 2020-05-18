using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SampleDynamoDbRepository;

namespace ConsoleDynamoDbRepository
{
    class Program
    {
        private static readonly string _tableName = "project-lambda-ItemsTable-97VLHR6QGBU";
        static async Task Main(string[] args)
        {
            // await TestProjectRepositoryCRUD();

            // await TestUserRepositoryCRUD();

            // await TestUserRepositoryAddItemsOneByOne();

            // Console.ReadKey();

            // await TestUserRepositoryReadOperations();

            // await TestUserRepositoryDeleteItemsOneByOne();

            // await TestUserRepositoryBatchAddItems();

            // Console.ReadKey();

            // await TestUserRepositoryReadOperations();

            // await TestUserRepositoryBatchDeleteItems();

            // await TestGameRepositorySpecificOperations();

            // await TestUserProjectRepositoryManyToMany();


            // await TestCRUD_UserRepo();


            await TestCRUD_GameRepo_GenericMethods();
        }

        private static async Task TestCRUD_GameRepo_GenericMethods()
        {
            var repo = new GameRepo(_tableName);
            var g1 = new Game { Id = "G1", Name = "Game G1" };
            Console.WriteLine("* Creating game G1");
            await repo.AddItemAsync("U1", "G1", g1);

            var g2 = new Game { Id = "G2", Name = "Game G2" };
            Console.WriteLine("* Creating game G2");
            await repo.AddItemAsync("U1", "G2", g2);

            Console.WriteLine("* Getting all users");
            var games = await repo.GetTableItemsByParentIdAsync("U1", "GAME");
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

        private static async Task TestCRUD_UserRepo()
        {
            var repo = new UserRepo(_tableName);

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

        private static async Task TestUserProjectRepositoryManyToMany()
        {
            var userRepo = new UserRepository(_tableName);

            var u1 = new User { Id = "U1", Name = "User 1", FirstName = "User", LastName = "A", Email = "a@test.com" };
            await userRepo.AddItemAsync(u1);


            var repo = new UserProjectRepository(_tableName);

            var u1p1 = new UserProject { UserId = "U1", ProjectId = "P1", Role = "owner" };
            await repo.AddItemAsync(u1p1);
            var u1p2 = new UserProject { UserId = "U1", ProjectId = "P2", Role = "member" };
            await repo.AddItemAsync(u1p2);

            Console.WriteLine("Getting projects by UserProject - should be empty");
            var allUP = await repo.GetAllItemsAsync();
            foreach (var item in allUP)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.WriteLine("Getting projects by user U1");
            var allUPU1 = await repo.GetTableItemsByParentIdAsync("U1");
            foreach (var item in allUPU1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }


            Console.WriteLine("Getting users by project P1");
            var usersP1 = await repo.GetUserProjectByProjectAsync("P1");
            foreach (var item in usersP1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
            Console.WriteLine("Getting users by project P2");
            var usersP2 = await repo.GetUserProjectByProjectAsync("P2");
            foreach (var item in usersP2)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }


            Console.WriteLine("Deleting projects P1 and P2 for user U1");
            await repo.DeleteProjectFromUserAsync(u1p1.UserId, u1p1.ProjectId);
            await repo.DeleteProjectFromUserAsync(u1p2.UserId, u1p2.ProjectId);

            Console.WriteLine("Getting projects by user U1 - should be empty");
            var deletedUPU1 = await repo.GetTableItemsByParentIdAsync("U1");
            foreach (var item in deletedUPU1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
        }

        private static async Task TestGameRepositorySpecificOperations()
        {
            var userId = "U1";
            var repo = new GameRepository(_tableName);

            for (int i = 0; i < 5; i++)
            {
                var g = new Game { Id = "GA" + i, Name = "Game A" + i };
                Console.WriteLine($"Adding {g.Id}");
                await repo.AddGameForUserAsync(userId, g);
            }

            Console.ReadKey();

            var games = await repo.GetGamesByUserAsync(userId);
            foreach (var item in games)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.ReadKey();

            for (int i = 0; i < 5; i++)
            {
                var gameId = "GA" + i;
                Console.WriteLine($"Deleting {gameId}");
                await repo.DeleteGameFromUserAsync(userId, gameId);
            }
        }

        private static async Task TestUserRepositoryDeleteItemsOneByOne()
        {
            var repo = new UserRepository(_tableName);
            for (int i = 0; i < 20; i++)
            {
                var uA = new User { Id = "A" + i };
                Console.WriteLine("* Deleting user A" + i);
                await repo.DeleteItemAsync(uA.Id);
            }
        }

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

        private static async Task TestUserRepositoryAddItemsOneByOne()
        {
            var repo = new UserRepository(_tableName);
            for (int i = 0; i < 20; i++)
            {
                var uA = new User { Id = "A" + i, Name = "userA" + i, FirstName = "User" + i, LastName = "A" + i, Email = $"a{i}@test.com" };
                Console.WriteLine("* Creating user A" + i);
                await repo.AddItemAsync(uA);
            }
        }

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

        private static async Task TestUserRepositoryReadOperations()
        {
            Console.WriteLine("***** Retrieving all items");
            var repo = new UserRepository(_tableName);
            var list = await repo.GetAllItemsAsync();
            foreach (var item in list)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.WriteLine("***** Retrieving total Count");
            Console.WriteLine($"{await repo.CountAsync()}");
        }

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

        private static async Task TestProjectRepositoryCRUD()
        {
            var repo = new ProjectRepository(_tableName);

            var pA = new Project { Id = "A", Name = "Project A", Description = "Desc proj A" };
            Console.WriteLine("* Creating project A");
            await repo.AddItemAsync(pA);

            Console.WriteLine("* Retrieving project A");
            var ppA = await repo.GetItemAsync("A");
            if (ppA != null)
                Console.WriteLine(JsonSerializer.Serialize(ppA));
            else
                Console.WriteLine("not found");

            Console.ReadKey();


            pA.Name = "Project AA";
            pA.Description = "Desc proj AA";
            Console.WriteLine("* Updating project A - renamed to AA");
            await repo.UpdateItemAsync(pA);

            Console.WriteLine("* Retrieving project A after update");
            var pAUpdated = await repo.GetItemAsync("A");
            if (pAUpdated != null)
                Console.WriteLine(JsonSerializer.Serialize(pAUpdated));
            else
                Console.WriteLine("not found");

            Console.ReadKey();

            Console.WriteLine("* Deleting project A");
            await repo.DeleteItemAsync("A");

            Console.WriteLine("* Retrieving project A after deletion");
            var deletedA = await repo.GetItemAsync("A");
            if (deletedA != null)
                Console.WriteLine(JsonSerializer.Serialize(deletedA));
            else
                Console.WriteLine("not found");
        }
    }
}
