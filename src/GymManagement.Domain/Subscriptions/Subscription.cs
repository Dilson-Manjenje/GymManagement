using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagement.Domain.Subscriptions
{
    public class Subscription
    {
        private Guid _adminId;
        public Guid Id { get; private set;  }
        public SubscriptionType SubscriptionType { get; private set; } = SubscriptionType.Basic;

        public Subscription(SubscriptionType subscriptionType, Guid adminId, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            _adminId = adminId;
            SubscriptionType = subscriptionType;
        }
        
        private Subscription()
        {
            SubscriptionType = SubscriptionType.Basic;
        }
    }
}