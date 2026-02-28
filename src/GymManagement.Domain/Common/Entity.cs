using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagement.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime? CreationDate { get; private set; } = null;
    public DateTime? LastUpdateDate { get; private set; } = null;
    [NotMapped]
    protected List<IDomainEvent> DomainEvents { get; private set; } = new();

    public Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
        
    }
    public void SetLastUpdate(DateTime now)
    {
        LastUpdateDate = now;
    }

    public void SetCreationDate(DateTime now)
    {
        if (now > DateTime.Now)
            throw new ArgumentException("Informed date is invalid.");

        CreationDate = now;
    }

    public IList<IDomainEvent> PopAndClearDomainEvents()
    {
        var copy = DomainEvents.ToList();
        DomainEvents.Clear();

        return copy;
    }
    
    public bool HasDomainEvents()
    {
        return  DomainEvents.Any();
    }
}