using System;
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
            var repo = new UserRepository(_tableName, _serviceUrl);

            var list = await repo.GetAllItemsAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new User { Id = "User0001", Name = "userA", FirstName = "User", LastName = "A", Email = "a@test.com" };
            await repo.AddItemAsync(item);

            list = await repo.GetAllItemsAsync();
            Assert.Equal(1, list.Count);

            count = await repo.CountAsync();
            Assert.Equal(1, count);

            var found = await repo.GetItemAsync("User0001");
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
            await repo.UpdateItemAsync(found);

            var updated = await repo.GetItemAsync("User0001");
            Assert.NotNull(updated);
            Assert.Equal("User0001", updated.Id);
            Assert.Equal("userAA", updated.Name);
            Assert.Equal("UserUser", updated.FirstName);
            Assert.Equal("AA AA", updated.LastName);
            Assert.Equal("aa-aa@test.com", updated.Email);

            await repo.DeleteItemAsync("User0001");
            var deleted = await repo.GetItemAsync("User0001");
            Assert.Null(deleted);

            var emptyList = await repo.GetAllItemsAsync();
            Assert.Equal(0, emptyList.Count);

            var emptyCount = await repo.CountAsync();
            Assert.Equal(0, emptyCount);
        }

        [Fact]
        public async void TestProjectRepository()
        {
            var repo = new ProjectRepository(_tableName, _serviceUrl);

            var list = await repo.GetAllItemsAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new Project { Id = "Project0001", Name = "ProjectA", Description = "Project A" };
            await repo.AddItemAsync(item);

            list = await repo.GetAllItemsAsync();
            Assert.Equal(1, list.Count);

            count = await repo.CountAsync();
            Assert.Equal(1, count);

            var found = await repo.GetItemAsync("Project0001");
            Assert.NotNull(found);
            Assert.Equal("Project0001", found.Id);
            Assert.Equal("ProjectA", found.Name);
            Assert.Equal("Project A", found.Description);

            found.Name = "ProjectAA";
            found.Description = "Project AA";
            await repo.UpdateItemAsync(found);

            var updated = await repo.GetItemAsync("Project0001");
            Assert.NotNull(updated);
            Assert.Equal("Project0001", updated.Id);
            Assert.Equal("ProjectAA", updated.Name);
            Assert.Equal("Project AA", updated.Description);


            await repo.DeleteItemAsync("Project0001");
            var deleted = await repo.GetItemAsync("Project0001");
            Assert.Null(deleted);

        }

        [Fact]
        public async void TestPersonRepository()
        {
            var repo = new PersonRepository(_tableName, _serviceUrl);

            var list = await repo.GetAllItemsAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new Person { Id = 1, Name = "personA", FirstName = "Person", LastName = "A", Email = "pa@test.com", };
            await repo.AddItemAsync(item);

            list = await repo.GetAllItemsAsync();
            Assert.Equal(1, list.Count);

            count = await repo.CountAsync();
            Assert.Equal(1, count);

            var found = await repo.GetItemAsync(1);
            Assert.NotNull(found);
            Assert.Equal(1, found.Id);
            Assert.Equal("personA", found.Name);
            Assert.Equal("Person", found.FirstName);
            Assert.Equal("A", found.LastName);
            Assert.Equal("pa@test.com", found.Email);


            found.Name = "personAA";
            found.FirstName = "PersonPerson";
            found.LastName = "AA AA";
            found.Email = "aa-aa@test.com";
            await repo.UpdateItemAsync(found);

            var updated = await repo.GetItemAsync(1);
            Assert.NotNull(updated);
            Assert.Equal(1, updated.Id);
            Assert.Equal("personAA", updated.Name);
            Assert.Equal("PersonPerson", updated.FirstName);
            Assert.Equal("AA AA", updated.LastName);
            Assert.Equal("aa-aa@test.com", updated.Email);

            await repo.DeleteItemAsync(1);
            var deleted = await repo.GetItemAsync(1);
            Assert.Null(deleted);
        }

        [Fact]
        public async void TestGameRepository()
        {
            var repo = new GameRepository(_tableName, _serviceUrl);
            var userId = "U1";

            var list0 = await repo.GetGamesByUserAsync(userId);
            Assert.Equal(0, list0.Count);

            var g = new Game { Id = "GA", Name = "Game A" };
            await repo.AddGameForUserAsync(userId, g);

            var list1 = await repo.GetGamesByUserAsync(userId);
            Assert.Equal(1, list1.Count);

            var found = await repo.GetItemAsync(userId, g.Id);
            Assert.NotNull(found);
            Assert.Equal("GA", found.Id);
            Assert.Equal("Game A", found.Name);

            await repo.DeleteGameFromUserAsync(userId, g.Id);

            var deleted = await repo.GetItemAsync(userId, g.Id);
            Assert.Null(deleted);
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
            var skPrefix = "TEST_ENTITY";

            var list = await repo.TableQueryItemsByParentIdAsync(parentId, skPrefix);
            Assert.Equal(0, list.Count);

            var te1 = new TestEntity { Id = "TE1", Name = "TestEntity TE1" };
            await repo.AddItemAsync(parentId, "TE1", te1);

            list = await repo.TableQueryItemsByParentIdAsync(parentId, skPrefix);
            Assert.Equal(1, list.Count);

            var te2 = new TestEntity { Id = "TE2", Name = "TestEntity TE2" };
            await repo.AddItemAsync(parentId, "TE2", te2);

            list = await repo.TableQueryItemsByParentIdAsync(parentId, skPrefix);
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

            list = await repo.TableQueryItemsByParentIdAsync(parentId, skPrefix);
            Assert.Equal(1, list.Count);

            var deleted1 = await repo.GetItemAsync(parentId, te1.Id);
            Assert.Null(deleted1);

            await repo.DeleteItemAsync(parentId, te2.Id);
            list = await repo.TableQueryItemsByParentIdAsync(parentId, skPrefix);
            Assert.Equal(0, list.Count);

            var deleted2 = await repo.GetItemAsync(parentId, te1.Id);
            Assert.Null(deleted2);
        }
    }
}
