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
            result.Id = item.GetStringValue("Id");
            result.Name = item.GetStringValue("Name");
            return result;
        }

        protected override DynamoDBItem ToDynamoDb(TestEntity item)
        {
            var dbItem = new DynamoDBItem();
            dbItem.AddStringValue("Id", item.Id);
            dbItem.AddStringValue("Name", item.Name);
            return dbItem;
        }
    }
}
