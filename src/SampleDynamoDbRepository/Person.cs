using DynamoCode.Domain.Entities;

namespace SampleDynamoDbRepository
{
    public class Person : Entity
    {
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}