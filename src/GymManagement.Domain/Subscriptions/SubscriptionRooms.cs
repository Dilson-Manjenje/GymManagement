using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Common;
using GymManagement.Domain.Rooms;

namespace GymManagement.Domain.Subscriptions;

public class SubscriptionRooms : Entity
{
    public Guid SubscriptionId { get; private set; }
    public Subscription Subscription { get; set; } = null!;
    public Guid RoomId { get; private set; }
    public Room Room { get; set; } = null!; // TODO: Review use of navigation property by business rules
    
    public int MaxSessionsPerMonth => Subscription.SubscriptionType.Name switch
    {
        nameof(SubscriptionType.Basic) => 7,
        nameof(SubscriptionType.Plus) => 15,
        nameof(SubscriptionType.Premium) => 70,
        _ => throw new InvalidCastException(SubscriptionErrors
                                                .InvalidSubscriptionType(Subscription.SubscriptionType.Name).Description)
    };        
        
    public SubscriptionRooms(Guid subscriptionId,
                             Guid roomId,
                             Guid? id = null)
            : base(id ?? Guid.NewGuid())
    {
        SubscriptionId = subscriptionId;
        RoomId = roomId;
    }

    protected SubscriptionRooms()
    {
    }    
}