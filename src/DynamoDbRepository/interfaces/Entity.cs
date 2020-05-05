
using System;
using System.Linq;

namespace DynamoCode.Domain.Entities
{
    [Serializable]
    public abstract class Entity<TId> : IEntityKey<TId>
    {
        public virtual TId Id { get; set; }
    }


    [Serializable]
    public abstract class Entity : Entity<int>
    {

    }
}
