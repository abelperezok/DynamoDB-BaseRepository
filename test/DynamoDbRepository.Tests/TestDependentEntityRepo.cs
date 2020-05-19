namespace DynamoDbRepository.Tests
{
    public class TestDependentEntityRepo : DependentEntityRepository<string, TestEntity>
    {
        public TestDependentEntityRepo(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "PARENT_ENTITY";
            SKPrefix = "TEST_ENTITY";
        }

        protected override TestEntity FromDynamoDb(DynamoDBItem item)
        {
            var result = new TestEntity();
            result.Id = item.GetString("Id");
            result.Name = item.GetString("Name");
            return result;
        }

        protected override DynamoDBItem ToDynamoDb(TestEntity item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddString("Id", item.Id);
            dbItem.AddString("Name", item.Name);
            return dbItem;
        }
    }
}
