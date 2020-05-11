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
            await TestProjectRepositoryCRUD();

            // await TestUserRepositoryCRUD();

            // await TestUserRepositoryAddItemsOneByOne();

            // Console.ReadKey();

            // await TestUserRepositoryReadOperations();

            // await TestUserRepositoryDeleteItemsOneByOne();

            // await TestUserRepositoryAddBatchItems();

            // Console.ReadKey();

            // await TestUserRepositoryReadOperations();

            // await TestUserRepositoryDeleteBatchItems();


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

        private static async Task TestUserRepositoryDeleteBatchItems()
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

        private static async Task TestUserRepositoryAddBatchItems()
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
            var list = await repo.GetAllAsync();
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
