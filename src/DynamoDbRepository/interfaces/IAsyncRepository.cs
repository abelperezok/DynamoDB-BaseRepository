namespace DynamoCode.Domain.Data.Interfaces
{
    public interface IAsyncRepository<TKey, TEntity> : IAsyncReadRepository<TKey, TEntity>, IAsyncWriteRepository<TKey, TEntity>
        where TEntity : class
    {
    }

    public interface IAsyncRepository<TEntity> : IAsyncReadRepository<TEntity>, IAsyncWriteRepository<TEntity> where TEntity : class
    {
    }
}
