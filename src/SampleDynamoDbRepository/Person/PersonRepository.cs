using DynamoDbRepository;

namespace SampleDynamoDbRepository
{
   public class PersonRepository : SimpleRepository<int, Person>
    {
        public PersonRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "PERSON";
            SKPrefix = "METADATA";
        }

        protected override int GetEntityKey(Person item)
        {
            return item.Id;
        }

        protected override DynamoDBItem ToDynamoDb(Person item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddNumber("Id", item.Id);
            dbItem.AddString("Name", item.Name);
            dbItem.AddString("Email", item.Email);
            dbItem.AddNumber("Age", item.Age);
            return dbItem;
        }

        protected override Person FromDynamoDb(DynamoDBItem item)
        {
            var result = new Person();
            result.Id = item.GetInt32("Id");
            result.Name = item.GetString("Name");
            result.Email = item.GetString("Email");
            result.Age = item.GetInt32("Age");
            return result;
        }
    }
}