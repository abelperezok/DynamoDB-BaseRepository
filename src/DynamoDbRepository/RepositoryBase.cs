using System;

namespace DynamoDbRepository
{
    public abstract class RepositoryBase<TKey, TEntity> where TEntity : class
    {
        protected string PKPrefix = "";
        protected string SKPrefix = "";
        protected string GSI1Prefix = "";

        protected string PKPattern { get { return $"{PKPrefix}{DynamoDBConstants.Separator}{{0}}"; } }
        protected string SKPattern { get { return $"{SKPrefix}{DynamoDBConstants.Separator}{{0}}"; } }
        protected string GSI1Pattern { get { return $"{GSI1Prefix}{DynamoDBConstants.Separator}{{0}}"; } }

        protected AmazonDynamoDBClientWrapper _dynamoDbClient;

        public RepositoryBase(string tableName, string serviceUrl = null)
        {
            _dynamoDbClient = new AmazonDynamoDBClientWrapper(tableName, serviceUrl);
        }

        protected abstract DynamoDBItem ToDynamoDb(TEntity item);
        protected abstract TEntity FromDynamoDb(DynamoDBItem item);

        protected string PKValue(object id)
        {
            return string.Format(PKPattern, Convert.ToString(id));
        }

        protected string SKValue(object id)
        {
            return string.Format(SKPattern, Convert.ToString(id));
        }

        protected string GSI1Value(object id)
        {
            return string.Format(GSI1Pattern, Convert.ToString(id));
        }
    }
}