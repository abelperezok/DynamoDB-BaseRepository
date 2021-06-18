using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DynamoDbRepository;
using Microsoft.Extensions.DependencyInjection;
using SampleDynamoDbRepository;

namespace ConsoleDynamoDbRepository
{
    class Program
    {
        private static readonly string _tableName = "dynamodb_test_table";
        static async Task Main(string[] args)
        {
            // await TestProjectRepositoryCRUD();

            // await TestUserRepositoryAddItemsOneByOne();

            // Console.ReadKey();


            // await TestUserRepositoryDeleteItemsOneByOne();

            // await TestUserRepositoryBatchAddItems();

            // await TestGameRepositoryBatchAddItems();

            // Console.ReadKey();


            // await TestUserRepositoryBatchDeleteItems();

            // await TestGameRepositoryBatchDeleteItems();

            // await TestGameRepositorySpecificOperations();

            // await TestUserProjectRepositoryManyToMany();


            // await TestCRUD_UserRepository();

            // await TestCRUD_GameRepo_GenericMethods();

            // await TestCRUD_PersonRepository();

            // DI example
            var serviceProvider = new ServiceCollection()
                // Add repositories via DI 
                .AddTransient<IUserRepository, UserRepository>(
                    x => new UserRepository(_tableName) // this value should maybe come from configuration
                )
                // Build the service provider
                .BuildServiceProvider(false);

            using (var scope = serviceProvider.CreateScope())
            {
                // Instantiate the Repo
                var userRepo = serviceProvider.GetRequiredService<IUserRepository>();

                // var users = await userRepo.GetUserList();
                // foreach (var item in users)
                // {
                //     Console.WriteLine(JsonSerializer.Serialize(item));
                // }

                await TestCRUD_UserRepository(userRepo);
            }
        }

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

        private static async Task TestCRUD_UserRepository(IUserRepository repo)
        {
            // IUserRepository repo = new UserRepository(_tableName);

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

        private static async Task TestUserProjectRepositoryManyToMany()
        {
            IUserRepository userRepo = new UserRepository(_tableName);

            var u1 = new User { Id = "U1", Name = "User 1", FirstName = "User", LastName = "A", Email = "a@test.com" };
            await userRepo.AddUser(u1);


            IUserProjectRepository repo = new UserProjectRepository(_tableName);

            var u1p1 = new UserProject { UserId = u1.Id, ProjectId = "P1", Role = "owner" };
            await repo.AddProjectToUser(u1p1);
            var u1p2 = new UserProject { UserId = u1.Id, ProjectId = "P2", Role = "member" };
            await repo.AddProjectToUser(u1p2);

            Console.WriteLine("Getting projects by user U1");
            var allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            foreach (var item in allUPU1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.WriteLine("Getting users by project P1");
            var usersP1 = await repo.GetUsersByProjectAsync("P1");
            foreach (var item in usersP1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
            Console.WriteLine("Getting users by project P2");
            var usersP2 = await repo.GetUsersByProjectAsync("P2");
            foreach (var item in usersP2)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.WriteLine("Deleting projects P1 and P2 for user U1");
            await repo.RemoveProjectFromUser(u1p1.UserId, u1p1.ProjectId);
            await repo.RemoveProjectFromUser(u1p2.UserId, u1p2.ProjectId);

            Console.WriteLine("Getting projects by user U1 - should be empty");
            var deletedUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            foreach (var item in deletedUPU1)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
        }

        private static async Task TestGameRepositorySpecificOperations()
        {
            var userId = "U1";
            IGameRepository repo = new GameRepository(_tableName);

            for (int i = 0; i < 5; i++)
            {
                var g = new Game { Id = "GA" + i, Name = "Game A" + i };
                Console.WriteLine($"Adding {g.Id}");
                await repo.AddGame(userId, g);
            }

            Console.ReadKey();

            var games = await repo.GetGameList(userId);
            foreach (var item in games)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.ReadKey();

            for (int i = 0; i < 5; i++)
            {
                var gameId = "GA" + i;
                Console.WriteLine($"Deleting {gameId}");
                await repo.DeleteGame(userId, gameId);
            }
        }

        private static async Task TestUserRepositoryDeleteItemsOneByOne()
        {
            IUserRepository repo = new UserRepository(_tableName);
            for (int i = 0; i < 20; i++)
            {
                var uA = new User { Id = "A" + i };
                Console.WriteLine("* Deleting user A" + i);
                await repo.DeleteUser(uA.Id);
            }
        }

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

        private static async Task TestUserRepositoryAddItemsOneByOne()
        {
            IUserRepository repo = new UserRepository(_tableName);
            for (int i = 0; i < 20; i++)
            {
                var uA = new User { Id = "A" + i, Name = "userA" + i, FirstName = "User" + i, LastName = "A" + i, Email = $"a{i}@test.com" };
                Console.WriteLine("* Creating user A" + i);
                await repo.AddUser(uA);
            }
        }

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

        private static async Task TestProjectRepositoryCRUD()
        {
            IProjectRepository repo = new ProjectRepository(_tableName);

            var pA = new Project { Id = "A", Name = "Project A", Description = "Desc proj A" };
            Console.WriteLine("* Creating project A");
            await repo.AddProject(pA);

            Console.WriteLine("* Retrieving project A");
            var ppA = await repo.GetProject("A");
            if (ppA != null)
                Console.WriteLine(JsonSerializer.Serialize(ppA));
            else
                Console.WriteLine("not found");

            Console.ReadKey();


            pA.Name = "Project AA";
            pA.Description = "Desc proj AA";
            Console.WriteLine("* Updating project A - renamed to AA");
            await repo.UpdateProject(pA);

            Console.WriteLine("* Retrieving project A after update");
            var pAUpdated = await repo.GetProject("A");
            if (pAUpdated != null)
                Console.WriteLine(JsonSerializer.Serialize(pAUpdated));
            else
                Console.WriteLine("not found");

            Console.ReadKey();

            Console.WriteLine("* Deleting project A");
            await repo.DeleteProject("A");

            Console.WriteLine("* Retrieving project A after deletion");
            var deletedA = await repo.GetProject("A");
            if (deletedA != null)
                Console.WriteLine(JsonSerializer.Serialize(deletedA));
            else
                Console.WriteLine("not found");
        }



    }
}
