using DynamoCode.Domain.Entities;

namespace SampleDynamoDbRepository
{
    public class User : Entity<string>
    {
        public virtual string Name { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual string CompleteName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
