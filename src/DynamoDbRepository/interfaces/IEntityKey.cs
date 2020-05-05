namespace DynamoCode.Domain.Entities
{
    public interface IEntityKey<TKey>
    {
        TKey Id { set; get; }
    }
}
