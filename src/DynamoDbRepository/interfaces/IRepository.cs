namespace DynamoCode.Domain.Data.Interfaces
{
    public interface IRepository<TKey, TEntity> : IReadRepository<TKey, TEntity>, IWriteRepository<TKey, TEntity>
        where TEntity : class
    {
    }

    public interface IRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
    {
    }
}
