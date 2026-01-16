using ErrorOr;
using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Domain.Members;

public class Member : Entity
{
    public Guid? UserId { get; private set; } = null;
    public string UserName { get; set; } = string.Empty;    
    public Guid? GymId { get; set; }
    public Gym? Gym { get; set; } = null; 
    
    // public Subscription? CurrentSubscription
    // {
    //     get {
    //         return Subscriptions?.SingleOrDefault( s => s.IsActive);             
    //     }
    
    // }    
   

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
     public ErrorOr<Success> Update(string? usarName = null,
                                    string? password = null)
    {
        UserName = usarName ?? UserName;
        
        return Result.Success;
    }
}