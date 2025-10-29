using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.SmartEnum;

namespace GymManagement.Domain.Subscriptions
{
    public class SubscriptionType: SmartEnum<SubscriptionType>
    {
        public static readonly SubscriptionType Basic = new (nameof(Basic), 1);
        public static readonly SubscriptionType Plus = new (nameof(Plus), 2);
        public static readonly SubscriptionType Premium = new(nameof(Premium), 3);
        public SubscriptionType(string name, int value) : base(name, value)
        {
        }
    }
}