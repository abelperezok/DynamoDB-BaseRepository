using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoCode.Domain.Data.Interfaces
{
    public interface IAsyncReadRepository<TKey, TEntity> where TEntity : class
    {
        Task<TEntity> FindByAsync(TKey id);

        Task<IList<TEntity>> AllAsync();

        Task<int> CountAsync();
    }

    public interface IAsyncReadRepository<TEntity> : IAsyncReadRepository<int, TEntity> where TEntity : class
    {

    }
}