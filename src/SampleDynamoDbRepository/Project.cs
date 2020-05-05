using DynamoCode.Domain.Entities;

namespace SampleDynamoDbRepository
{
    public class Project : Entity<string>
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

    }
}
