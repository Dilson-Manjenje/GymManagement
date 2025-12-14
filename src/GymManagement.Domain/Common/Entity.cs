namespace GymManagement.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTime? CreationDate { get; private set; } = null;
    public DateTime? LastUpdateDate { get; private set; } = null;

    public Entity(Guid id)
    {
        Id = id;
    }

    public void SetLastUpdate(DateTime utcNow)
    {
        LastUpdateDate = utcNow;
    }

    public void SetCreationDate(DateTime utcNow)
    {
        if (utcNow > DateTime.Now)
            throw new ArgumentException("Informed date is invalid.");

        CreationDate = utcNow;
    }
    
    protected Entity()
    {

    }
}