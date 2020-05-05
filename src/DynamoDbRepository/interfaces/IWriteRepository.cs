using System.Collections.Generic;

namespace DynamoCode.Domain.Data.Interfaces
{
    public interface IWriteRepository<TKey, TEntity> where TEntity : class
    {
        void Add(TEntity item);

        void Add(IEnumerable<TEntity> items);

        void Update(TEntity item);

        void Delete(TEntity item);

        void Delete(TKey id);

        void Delete(IEnumerable<TEntity> items);
    }

    public interface IWriteRepository<TEntity> : IWriteRepository<int, TEntity> where TEntity : class
    {

    }
}
