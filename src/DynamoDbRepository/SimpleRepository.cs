using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDbRepository
{
    public abstract class SimpleRepository<TKey, TEntity> : IndependentEntityRepository<TKey, TEntity>, ISimpleRepository<TKey, TEntity>
        where TEntity : class
    {
        public SimpleRepository(string tableName, string serviceUrl = null) : base(tableName, serviceUrl)
        {
        }

        protected abstract TKey GetEntityKey(TEntity item);

        public async Task Add(TEntity item)
        {
            var key = GetEntityKey(item);
            await AddItemAsync(key, item);
        }

        public async Task Delete(TKey key)
        {
            await DeleteItemAsync(key);
        }

        public async Task<IList<TEntity>> GetList()
        {
            return await GSI1QueryAllAsync();
        }

        public async Task Update(TEntity item)
        {
            var key = GetEntityKey(item);
            await AddItemAsync(key, item);
        }

        public async Task<TEntity> Get(TKey key)
        {
            return await GetItemAsync(key);
        }
    }
}