using System;
using System.Collections.Generic;
using System.Text.Json;
using SampleDynamoDbRepository;
using Xunit;

namespace DynamoDbRepository.Tests
{

    public class RepositoryIntegrationTest : IClassFixture<DynamoDBDockerFixture>
    {
        private string _serviceUrl;
        private string _tableName;

        public RepositoryIntegrationTest(DynamoDBDockerFixture fixture)
        {
            _serviceUrl = fixture.ServiceUrl;
            _tableName = fixture.TableName;
        }

        [Fact]
        public async void TestRepo_UserRepository()
        {
            IUserRepository repo = new UserRepository(_tableName, _serviceUrl);

            var list = await repo.GetUserList();
            Assert.Equal(0, list.Count);

            var item = new User { Id = "User0001", Name = "userA", FirstName = "User", LastName = "A", Email = "a@test.com" };
            await repo.AddUser(item);

            list = await repo.GetUserList();
            Assert.Equal(1, list.Count);

            var item0 = list[0];
            Assert.NotNull(item0);
            Assert.Equal("User0001", item0.Id);
            Assert.Equal("userA", item0.Name);
            Assert.Equal("User", item0.FirstName);
            Assert.Equal("A", item0.LastName);
            Assert.Equal("a@test.com", item0.Email);

            var found = await repo.GetUser("User0001");
            Assert.NotNull(found);
            Assert.Equal("User0001", found.Id);
            Assert.Equal("userA", found.Name);
            Assert.Equal("User", found.FirstName);
            Assert.Equal("A", found.LastName);
            Assert.Equal("a@test.com", found.Email);


            found.Name = "userAA";
            found.Email = "aa-aa@test.com";
            found.FirstName = "UserUser";
            found.LastName = "AA AA";
            await repo.UpdateUser(found);

            var updated = await repo.GetUser("User0001");
            Assert.NotNull(updated);
            Assert.Equal("User0001", updated.Id);
            Assert.Equal("userAA", updated.Name);
            Assert.Equal("UserUser", updated.FirstName);
            Assert.Equal("AA AA", updated.LastName);
            Assert.Equal("aa-aa@test.com", updated.Email);

            await repo.DeleteUser("User0001");
            var deleted = await repo.GetUser("User0001");
            Assert.Null(deleted);

            var emptyList = await repo.GetUserList();
            Assert.Equal(0, emptyList.Count);
        }

        [Fact]
        public async void TestRepo_Batch_UserRepository()
        {

            IUserRepository repo = new UserRepository(_tableName, _serviceUrl);

            var itemsToCreate = new List<User>();
            for (int i = 50; i < 60; i++)
            {
                var u = new User { Id = "A" + i, Name = "userA" + i, FirstName = "User" + i, LastName = "A" + i, Email = $"a{i}@test.com" };
                itemsToCreate.Add(u);
            }
            await repo.BatchAddUsers(itemsToCreate);

            var list = await repo.GetUserList();
            Assert.Equal(10, list.Count);

            for (int i = 0; i < 10; i++)
            {
                var item = list[i];
                var id = i + 50;
                Assert.NotNull(item);
                Assert.Equal("A" + id, item.Id);
                Assert.Equal("userA" + id, item.Name);
                Assert.Equal("User" + id, item.FirstName);
                Assert.Equal("A" + id, item.LastName);
                Assert.Equal($"a{id}@test.com", item.Email);
            }

            var itemsToDelete = new List<User>();
            for (int i = 50; i < 60; i++)
            {
                var up = new User { Id = "A" + i };
                itemsToDelete.Add(up);
            }
            await repo.BatchDeleteUsers(itemsToDelete);

            var emptyList = await repo.GetUserList();
            Assert.Empty(emptyList);
        }

        [Fact]
        public async void TestRepo_ProjectRepository()
        {
            IProjectRepository repo = new ProjectRepository(_tableName, _serviceUrl);

            var list = await repo.GetProjectList();
            Assert.Equal(0, list.Count);

            var item = new Project { Id = "Project0001", Name = "ProjectA", Description = "Project A" };
            await repo.AddProject(item);

            list = await repo.GetProjectList();
            Assert.Equal(1, list.Count);

            var item0 = list[0];
            Assert.NotNull(item0);
            Assert.Equal("Project0001", item0.Id);
            Assert.Equal("ProjectA", item0.Name);
            Assert.Equal("Project A", item0.Description);

            var found = await repo.GetProject("Project0001");
            Assert.NotNull(found);
            Assert.Equal("Project0001", found.Id);
            Assert.Equal("ProjectA", found.Name);
            Assert.Equal("Project A", found.Description);

            found.Name = "ProjectAA";
            found.Description = "Project AA";
            await repo.UpdateProject(found);

            var updated = await repo.GetProject("Project0001");
            Assert.NotNull(updated);
            Assert.Equal("Project0001", updated.Id);
            Assert.Equal("ProjectAA", updated.Name);
            Assert.Equal("Project AA", updated.Description);


            await repo.DeleteProject("Project0001");
            var deleted = await repo.GetProject("Project0001");
            Assert.Null(deleted);

        }

        [Fact]
        public async void TestRepo_PersonRepository()
        {
            ISimpleRepository<int, Person> repo = new PersonRepository(_tableName, _serviceUrl);

            var list = await repo.GetList();
            Assert.Equal(0, list.Count);

            var item = new Person { Id = 1, Name = "personA", Email = "pa@test.com", Age = 35, Height = 1.75 };
            await repo.Add(item);

            list = await repo.GetList();
            Assert.Equal(1, list.Count);

            var item0 = list[0];
            Assert.NotNull(item0);
            Assert.Equal(1, item0.Id);
            Assert.Equal("personA", item0.Name);
            Assert.Equal("pa@test.com", item0.Email);
            Assert.Equal(35, item0.Age);
            Assert.Equal(1.75, item0.Height);

            var found = await repo.Get(1);
            Assert.NotNull(found);
            Assert.Equal(1, found.Id);
            Assert.Equal("personA", found.Name);
            Assert.Equal("pa@test.com", found.Email);
            Assert.Equal(35, found.Age);
            Assert.Equal(1.75, found.Height);

            found.Name = "personAA";
            found.Email = "aa-aa@test.com";
            found.Age = 36;
            found.Height = 1.78;
            await repo.Update(found);

            var updated = await repo.Get(1);
            Assert.NotNull(updated);
            Assert.Equal(1, updated.Id);
            Assert.Equal("personAA", updated.Name);
            Assert.Equal("aa-aa@test.com", updated.Email);
            Assert.Equal(36, updated.Age);
            Assert.Equal(1.78, updated.Height);

            await repo.Delete(1);
            var deleted = await repo.Get(1);
            Assert.Null(deleted);

            var emptyList = await repo.GetList();
            Assert.Equal(0, emptyList.Count);
        }

        [Fact]
        public async void TestRepo_GameRepository()
        {
            IGameRepository repo = new GameRepository(_tableName, _serviceUrl);
            var userId = "U1";

            var list0 = await repo.GetGameList(userId);
            Assert.Equal(0, list0.Count);

            var g = new Game { Id = "GA", Name = "Game A" };
            await repo.AddGame(userId, g);

            var list1 = await repo.GetGameList(userId);
            Assert.Equal(1, list1.Count);

            var item0 = list1[0];
            Assert.NotNull(item0);
            Assert.Equal("GA", item0.Id);
            Assert.Equal("Game A", item0.Name);

            var found = await repo.GetGame(userId, g.Id);
            Assert.NotNull(found);
            Assert.Equal("GA", found.Id);
            Assert.Equal("Game A", found.Name);

            found.Name = "Game AA";
            await repo.UpdateGame(userId, found);

            var updated = await repo.GetGame(userId, g.Id);
            Assert.NotNull(found);
            Assert.Equal("Game AA", found.Name);

            await repo.DeleteGame(userId, g.Id);

            var deleted = await repo.GetGame(userId, g.Id);
            Assert.Null(deleted);

            var emptyList = await repo.GetGameList(userId);
            Assert.Equal(0, emptyList.Count);
        }

        [Fact]
        public async void TestRepo_Batch_GameRepository()
        {
            var u1 = new User { Id = "U1", Name = "User 1", FirstName = "User", LastName = "A", Email = "a@test.com" };

            IGameRepository repo = new GameRepository(_tableName, _serviceUrl);

            var itemsToCreate = new List<Game>();
            for (int i = 50; i < 60; i++)
            {
                var g = new Game { Id = "G" + i, Name = "Game " + i };
                itemsToCreate.Add(g);
            }
            await repo.BatchAddGames(u1.Id, itemsToCreate);

            var list = await repo.GetGameList(u1.Id);
            Assert.Equal(10, list.Count);

            for (int i = 0; i < 10; i++)
            {
                var item = list[i];
                var id = i + 50;
                Assert.NotNull(item);
                Assert.Equal("G" + id, item.Id);
                Assert.Equal("Game " + id, item.Name);
            }

            var itemsToDelete = new List<Game>();
            for (int i = 50; i < 60; i++)
            {
                var g = new Game { Id = "G" + i, Name = "Game " + i };
                itemsToDelete.Add(g);
            }
            await repo.BatchDeleteGames(u1.Id, itemsToDelete);

            var emptyList = await repo.GetGameList(u1.Id);
            Assert.Empty(emptyList);
        }

        [Fact]
        public async void TestRepo_UserProjectRepository()
        {
            var u1 = new User { Id = "U1", Name = "User 1", FirstName = "User", LastName = "A", Email = "a@test.com" };

            IUserProjectRepository repo = new UserProjectRepository(_tableName, _serviceUrl);

            var allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(0, allUPU1.Count);

            var u1p1 = new UserProject { UserId = u1.Id, ProjectId = "P1", Role = "owner" };
            await repo.AddProjectToUser(u1p1);

            allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(1, allUPU1.Count);

            var u1p2 = new UserProject { UserId = u1.Id, ProjectId = "P2", Role = "member" };
            await repo.AddProjectToUser(u1p2);

            allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(2, allUPU1.Count);
            Assert.Equal(u1.Id, allUPU1[0].UserId);
            Assert.Equal("P1", allUPU1[0].ProjectId);
            Assert.Equal("owner", allUPU1[0].Role);
            Assert.Equal(u1.Id, allUPU1[1].UserId);
            Assert.Equal("P2", allUPU1[1].ProjectId);
            Assert.Equal("member", allUPU1[1].Role);

            var usersP1 = await repo.GetUsersByProjectAsync("P1");
            Assert.Equal(1, usersP1.Count);
            Assert.Equal(u1.Id, usersP1[0].UserId);
            Assert.Equal("P1", usersP1[0].ProjectId);
            Assert.Equal("owner", usersP1[0].Role);

            var usersP2 = await repo.GetUsersByProjectAsync("P2");
            Assert.Equal(1, usersP2.Count);
            Assert.Equal(u1.Id, usersP2[0].UserId);
            Assert.Equal("P2", usersP2[0].ProjectId);
            Assert.Equal("member", usersP2[0].Role);

            await repo.RemoveProjectFromUser(u1p1.UserId, u1p1.ProjectId);
            allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(1, allUPU1.Count);

            await repo.RemoveProjectFromUser(u1p2.UserId, u1p2.ProjectId);
            allUPU1 = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(0, allUPU1.Count);
        }

        [Fact]
        public async void TestRepo_Batch_UserProjectRepository()
        {
            var u1 = new User { Id = "U1", Name = "User 1", FirstName = "User", LastName = "A", Email = "a@test.com" };

            IUserProjectRepository repo = new UserProjectRepository(_tableName, _serviceUrl);

            var itemsToCreate = new List<UserProject>();
            for (int i = 50; i < 60; i++)
            {
                var up = new UserProject { UserId = u1.Id, ProjectId = "P" + i, Role = "member" };
                itemsToCreate.Add(up);
            }
            await repo.BatchAddProjectsToUser(u1.Id, itemsToCreate);

            var list = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Equal(10, list.Count);

            for (int i = 0; i < 10; i++)
            {
                var item = list[i];
                var id = i + 50;
                Assert.NotNull(item);
                Assert.Equal(u1.Id, item.UserId);
                Assert.Equal("P" + id, item.ProjectId);
                Assert.Equal("member", item.Role);
            }

            var itemsToDelete = new List<UserProject>();
            for (int i = 50; i < 60; i++)
            {
                var up = new UserProject { UserId = u1.Id, ProjectId = "P" + i };
                itemsToDelete.Add(up);
            }
            await repo.BatchRemoveProjectsFromUser(u1.Id, itemsToDelete);

            var emptyList = await repo.GetProjectsByUserAsync(u1.Id);
            Assert.Empty(emptyList);
        }

        [Fact]
        public async void TestGenericOperations_IndependentEntityRepository()
        {
            var repo = new TestIndependentEntityRepo(_tableName, _serviceUrl);

            var list = await repo.GSI1QueryAllAsync();
            Assert.Equal(0, list.Count);

            var te1 = new TestEntity { Id = "TE1", Name = "TestEntity TE1" };
            await repo.AddItemAsync("TE1", te1);

            list = await repo.GSI1QueryAllAsync();
            Assert.Equal(1, list.Count);

            var te2 = new TestEntity { Id = "TE2", Name = "TestEntity TE2" };
            await repo.AddItemAsync("TE2", te2);

            list = await repo.GSI1QueryAllAsync();
            Assert.Equal(2, list.Count);

            var found1 = await repo.GetItemAsync(te1.Id);
            Assert.NotNull(found1);
            Assert.Equal("TE1", found1.Id);
            Assert.Equal("TestEntity TE1", found1.Name);

            var found2 = await repo.GetItemAsync(te2.Id);
            Assert.NotNull(found2);
            Assert.Equal("TE2", found2.Id);
            Assert.Equal("TestEntity TE2", found2.Name);

            await repo.DeleteItemAsync(te1.Id);

            list = await repo.GSI1QueryAllAsync();
            Assert.Equal(1, list.Count);

            var deleted1 = await repo.GetItemAsync(te1.Id);
            Assert.Null(deleted1);

            await repo.DeleteItemAsync(te2.Id);
            list = await repo.GSI1QueryAllAsync();
            Assert.Equal(0, list.Count);

            var deleted2 = await repo.GetItemAsync(te1.Id);
            Assert.Null(deleted2);
        }

        [Fact]
        public async void TestGenericOperations_DependentEntityRepository()
        {
            var repo = new TestDependentEntityRepo(_tableName, _serviceUrl);
            var parentId = "P0001";

            var list = await repo.TableQueryItemsByParentIdAsync(parentId);
            Assert.Equal(0, list.Count);

            var te1 = new TestEntity { Id = "TE1", Name = "TestEntity TE1" };
            await repo.AddItemAsync(parentId, "TE1", te1);

            list = await repo.TableQueryItemsByParentIdAsync(parentId);
            Assert.Equal(1, list.Count);

            var te2 = new TestEntity { Id = "TE2", Name = "TestEntity TE2" };
            await repo.AddItemAsync(parentId, "TE2", te2);

            list = await repo.TableQueryItemsByParentIdAsync(parentId);
            Assert.Equal(2, list.Count);

            var found1 = await repo.GetItemAsync(parentId, te1.Id);
            Assert.NotNull(found1);
            Assert.Equal("TE1", found1.Id);
            Assert.Equal("TestEntity TE1", found1.Name);

            var found2 = await repo.GetItemAsync(parentId, te2.Id);
            Assert.NotNull(found2);
            Assert.Equal("TE2", found2.Id);
            Assert.Equal("TestEntity TE2", found2.Name);

            await repo.DeleteItemAsync(parentId, te1.Id);

            list = await repo.TableQueryItemsByParentIdAsync(parentId);
            Assert.Equal(1, list.Count);

            var deleted1 = await repo.GetItemAsync(parentId, te1.Id);
            Assert.Null(deleted1);

            await repo.DeleteItemAsync(parentId, te2.Id);
            list = await repo.TableQueryItemsByParentIdAsync(parentId);
            Assert.Equal(0, list.Count);

            var deleted2 = await repo.GetItemAsync(parentId, te1.Id);
            Assert.Null(deleted2);
        }

        [Fact]
        public async void TestBatchOperations_IndependentEntityRepository()
        {
            var repo = new TestIndependentEntityRepo(_tableName, _serviceUrl);
            var itemsToCreate = new List<KeyValuePair<string, TestEntity>>();
            for (int i = 50; i < 60; i++)
            {
                var te = new TestEntity { Id = "TE" + i, Name = "TestEntity TE" + i };
                itemsToCreate.Add(new KeyValuePair<string, TestEntity>(te.Id, te));
            }
            await repo.BatchAddItemsAsync(itemsToCreate);

            var list = await repo.GSI1QueryAllAsync();
            Assert.Equal(10, list.Count);

            for (int i = 0; i < 10; i++)
            {
                var item = list[i];
                var id = i + 50;
                Assert.NotNull(item);
                Assert.Equal("TE" + id, item.Id);
                Assert.Equal("TestEntity TE" + id, item.Name);
            }

            var itemsToDelete = new List<string>();
            for (int i = 50; i < 60; i++)
            {
                var te = new TestEntity { Id = "TE" + i };
                itemsToDelete.Add(te.Id);
            }
            await repo.BatchDeleteItemsAsync(itemsToDelete);

            var emptyList = await repo.GSI1QueryAllAsync();
            Assert.Empty(emptyList);
        }

        [Fact]
        public async void TestBatchOperations_DependentEntityRepository()
        {
            var parent = new TestEntity { Id = "PTE1" };
            var repo = new TestDependentEntityRepo(_tableName, _serviceUrl);
            var itemsToCreate = new List<KeyValuePair<string, TestEntity>>();
            for (int i = 50; i < 60; i++)
            {
                var te = new TestEntity { Id = "TE" + i, Name = "TestEntity TE" + i };
                itemsToCreate.Add(new KeyValuePair<string, TestEntity>(te.Id, te));
            }
            await repo.BatchAddItemsAsync(parent.Id, itemsToCreate);

            var list = await repo.TableQueryItemsByParentIdAsync(parent.Id);
            Assert.Equal(10, list.Count);

            for (int i = 0; i < 10; i++)
            {
                var item = list[i];
                var id = i + 50;
                Assert.NotNull(item);
                Assert.Equal("TE" + id, item.Id);
                Assert.Equal("TestEntity TE" + id, item.Name);
            }

            var itemsToDelete = new List<string>();
            for (int i = 50; i < 60; i++)
            {
                var te = new TestEntity { Id = "TE" + i };
                itemsToDelete.Add(te.Id);
            }
            await repo.BatchDeleteItemsAsync(parent.Id, itemsToDelete);

            var emptyList = await repo.TableQueryItemsByParentIdAsync(parent.Id);
            Assert.Empty(emptyList);
        }
    }
}
