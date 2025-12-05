using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Admins;

namespace GymManagement.Domain.Subscriptions
{
    public class Subscription
    {
        public Guid Id { get; private set;  }
        public SubscriptionType SubscriptionType { get; private set; } = SubscriptionType.Basic;
        public Guid AdminId { get; private set; }
        public Admin Admin { get; set; } = null!;
        public int DurationInDays { get; } = 30;        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        public bool IsActive
        {
            get { return EndDate >= DateTime.Today; }            
        }

        public Subscription(SubscriptionType subscriptionType, Guid adminId, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            AdminId = adminId;
            SubscriptionType = subscriptionType;
            StartDate = DateTime.Now;
            EndDate = StartDate.AddDays(DurationInDays);
        }
        
        private Subscription()
        {
            SubscriptionType = SubscriptionType.Basic;
        }

        public ErrorOr<Updated> UpdateSubscription(SubscriptionType subscriptionType)
        {
            SubscriptionType = subscriptionType;
            
            return Result.Updated;
        }
       
        public int GetMaxRooms() => SubscriptionType.Name switch
        {
            nameof(SubscriptionType.Basic) => 1,
            nameof(SubscriptionType.Plus) => 2,
            nameof(SubscriptionType.Premium) => int.MaxValue,
            _ => throw new InvalidCastException(SubscriptionErrors.InvalidSubscriptionType(SubscriptionType.Name).Description)
        };

        public int Price => SubscriptionType.Name switch
        {
            nameof(SubscriptionType.Basic) => 20000,
            nameof(SubscriptionType.Plus) => 30000,
            nameof(SubscriptionType.Premium) => 50000,
            _ => throw new InvalidCastException(SubscriptionErrors.InvalidSubscriptionType(SubscriptionType.Name).Description)
        };

        public int MaxDailySessions => SubscriptionType.Name switch
        {
            nameof(SubscriptionType.Basic) => 1,
            nameof(SubscriptionType.Plus) => 3,
            nameof(SubscriptionType.Premium) => int.MaxValue,
            _ => throw new InvalidCastException(SubscriptionErrors.InvalidSubscriptionType(SubscriptionType.Name).Description)
        };        
    }
}