using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Trainers;

namespace GymManagement.Domain.Members;

public class Member : Entity
{
    public Guid? UserId { get; private set; } = null;
    public string UserName { get; set; } = string.Empty;
    
    public Subscription? CurrentSubscription
    {
        get {
            return Subscriptions?.SingleOrDefault( s => s.IsActive);             
        }
    
    }
    
    public List<Subscription>? Subscriptions { get; set; } = new();     
    public Trainer? Trainer { get; set; } = null; // Navigation for 1:1    
    public Guid? GymId { get; set; }
    public Gym? Gym { get; set; } = null; 


    public Member(
        string userName,
        Guid? gymId = null,
        Guid? userId = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserName = userName;
        GymId = gymId;
        UserId = userId ?? Guid.NewGuid();        
    }

    private Member() { }

    public bool HasActiveSubscription()
    {
        if (Subscriptions is null)
            return false;

        return Subscriptions.Any(s => s.IsActive);        
    }    
}