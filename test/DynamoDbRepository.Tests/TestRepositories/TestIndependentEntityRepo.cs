namespace DynamoDbRepository.Tests
{
    public class TestIndependentEntityRepo : IndependentEntityRepository<string, TestEntity>
    {
        public TestIndependentEntityRepo(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
            PKPrefix = "TEST_ENTITY";
            SKPrefix = "METADATA";
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
