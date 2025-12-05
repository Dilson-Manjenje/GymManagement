using ErrorOr;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Trainers;
using Throw;

namespace GymManagement.Domain.Admins;

public class Admin
{
    public Guid Id { get; private set; }
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


    public Admin(
        string userName,
        Guid? gymId = null,
        Guid? userId = null,
        Guid? id = null)
    {
        UserName = userName;
        GymId = gymId;
        UserId = userId ?? Guid.NewGuid();
        Id = id ?? Guid.NewGuid();
    }

    private Admin() { }

    public bool HasActiveSubscription()
    {
        if (Subscriptions is null)
            return false;
            
        return Subscriptions.Any(s => s.IsActive);
    }    
}