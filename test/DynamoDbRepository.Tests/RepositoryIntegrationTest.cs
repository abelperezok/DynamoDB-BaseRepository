﻿using System;
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
        public async void TestUserRepository()
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
        public async void TestProjectRepository()
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
        public async void TestPersonRepository()
        {
            ISimpleRepository<int, Person> repo = new PersonRepository(_tableName, _serviceUrl);

            var list = await repo.GetList();
            Assert.Equal(0, list.Count);

            var item = new Person { Id = 1, Name = "personA", Email = "pa@test.com", Age = 35 };
            await repo.Add(item);

            list = await repo.GetList();
            Assert.Equal(1, list.Count);

            var item0 = list[0];
            Assert.NotNull(item0);
            Assert.Equal(1, item0.Id);
            Assert.Equal("personA", item0.Name);
            Assert.Equal("pa@test.com", item0.Email);
            Assert.Equal(35, item0.Age);

            var found = await repo.Get(1);
            Assert.NotNull(found);
            Assert.Equal(1, found.Id);
            Assert.Equal("personA", found.Name);
            Assert.Equal("pa@test.com", found.Email);
            Assert.Equal(35, found.Age);

            found.Name = "personAA";
            found.Email = "aa-aa@test.com";
            found.Age = 36;
            await repo.Update(found);

            var updated = await repo.Get(1);
            Assert.NotNull(updated);
            Assert.Equal(1, updated.Id);
            Assert.Equal("personAA", updated.Name);
            Assert.Equal("aa-aa@test.com", updated.Email);
            Assert.Equal(36, updated.Age);

            await repo.Delete(1);
            var deleted = await repo.Get(1);
            Assert.Null(deleted);

            var emptyList = await repo.GetList();
            Assert.Equal(0, emptyList.Count);
        }

        [Fact]
        public async void TestGameRepository()
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
        public async void TestGenericIndependentEntityRepository()
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
        public async void TestGenericDependentEntityRepository()
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
    }
}
