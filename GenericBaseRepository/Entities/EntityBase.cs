using System.Security.Principal;

namespace GenericBaseRepository.Entities;

public abstract class EntityBase<TId>: IEntity<TId>, IEntityTimestamps, IEntityAuthorizeds
{
    public TId Id { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; } = default!;

    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public int Version { get; set; } 

    public EntityBase()
    {
        Id = default!;
    }

    public EntityBase(TId id)
    {
        Id = id;
    }
}
