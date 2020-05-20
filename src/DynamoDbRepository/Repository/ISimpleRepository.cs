using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDbRepository
{
    public interface ISimpleRepository<TKey, TEntity> where TEntity : class
    {
        Task Add(TEntity item);
        Task Delete(TKey key);
        Task<TEntity> Get(TKey key);
        Task<IList<TEntity>> GetList();
        Task Update(TEntity item);
    }
}