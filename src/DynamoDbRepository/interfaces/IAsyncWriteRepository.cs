using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoCode.Domain.Data.Interfaces
{
    public interface IAsyncWriteRepository<TKey, TEntity> where TEntity : class
    {
        Task AddAsync(TEntity item);

        Task AddAsync(IEnumerable<TEntity> items);

        Task UpdateAsync(TEntity item);

        Task DeleteAsync(TEntity item);

        Task DeleteAsync(TKey id);

        Task DeleteAsync(IEnumerable<TEntity> items);
    }

    public interface IAsyncWriteRepository<TEntity> : IAsyncWriteRepository<int, TEntity> where TEntity : class
    {

    }
}