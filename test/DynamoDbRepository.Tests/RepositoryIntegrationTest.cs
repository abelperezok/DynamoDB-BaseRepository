using System;
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

            var list = await repo.AllAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new User { Id = "User0001", Name = "userA", FirstName = "User", LastName = "A", Email = "a@test.com" };
            await repo.AddAsync(item);

            list = await repo.AllAsync();
            Assert.Equal(1, list.Count);

            count = await repo.CountAsync();
            Assert.Equal(1, count);

            var found = await repo.FindByAsync("User0001");
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
            await repo.UpdateAsync(found);

            var updated = await repo.FindByAsync("User0001");
            Assert.NotNull(updated);
            Assert.Equal("User0001", updated.Id);
            Assert.Equal("userAA", updated.Name);
            Assert.Equal("UserUser", updated.FirstName);
            Assert.Equal("AA AA", updated.LastName);
            Assert.Equal("aa-aa@test.com", updated.Email);

            await repo.DeleteAsync("User0001");
            var deleted = await repo.FindByAsync("User0001");
            Assert.Null(deleted);

            var emptyList = await repo.AllAsync();
            Assert.Equal(0, emptyList.Count);

            var emptyCount = await repo.CountAsync();
            Assert.Equal(0, emptyCount);
        }

        [Fact]
        public async void TestProjectRepository()
        {
            var repo = new ProjectRepository(_tableName, _serviceUrl);

            var list = await repo.AllAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new Project { Id = "Project0001", Name = "ProjectA", Description = "Project A" };
            await repo.AddAsync(item);

            var found = await repo.FindByAsync("Project0001");
            Assert.NotNull(found);
            Assert.Equal("Project0001", found.Id);
            Assert.Equal("ProjectA", found.Name);
            Assert.Equal("Project A", found.Description);

            found.Name = "ProjectAA";
            found.Description = "Project AA";
            await repo.UpdateAsync(found);

            var updated = await repo.FindByAsync("Project0001");
            Assert.NotNull(updated);
            Assert.Equal("Project0001", updated.Id);
            Assert.Equal("ProjectAA", updated.Name);
            Assert.Equal("Project AA", updated.Description);


            await repo.DeleteAsync("Project0001");
            var deleted = await repo.FindByAsync("Project0001");
            Assert.Null(deleted);

        }

        [Fact]
        public async void TestPersonRepository()
        {
            var repo = new PersonRepository(_tableName, _serviceUrl);

            var list = await repo.AllAsync();
            Assert.Equal(0, list.Count);


            var count = await repo.CountAsync();
            Assert.Equal(0, count);


            var item = new Person { Id = 1, Name = "personA", FirstName = "Person", LastName = "A", Email = "pa@test.com", };
            await repo.AddAsync(item);

            var found = await repo.FindByAsync(1);
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
            await repo.UpdateAsync(found);

            var updated = await repo.FindByAsync(1);
            Assert.NotNull(updated);
            Assert.Equal(1, updated.Id);
            Assert.Equal("personAA", updated.Name);
            Assert.Equal("PersonPerson", updated.FirstName);
            Assert.Equal("AA AA", updated.LastName);
            Assert.Equal("aa-aa@test.com", updated.Email);

            await repo.DeleteAsync(1);
            var deleted = await repo.FindByAsync(1);
            Assert.Null(deleted);
        }

    }
}
