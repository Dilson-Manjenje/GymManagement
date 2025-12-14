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

        public int DurationInDays => SubscriptionTypeConst.DurationInDays;
        public int MaxRooms => Name switch
        {
            nameof(Basic) => SubscriptionTypeConst.Basic.MaxRooms,
            nameof(Plus) => SubscriptionTypeConst.Plus.MaxRooms,
            nameof(Premium) => SubscriptionTypeConst.Premium.MaxRooms,
            _ => throw new ArgumentException()
        };        

        public decimal Price => Name switch
        {
            nameof(Basic) => SubscriptionTypeConst.Basic.Price,
            nameof(Plus) => SubscriptionTypeConst.Plus.Price,
            nameof(Premium) => SubscriptionTypeConst.Premium.Price,
            _ => throw new InvalidCastException()
        };

        public int MaxDailySessions => Name switch
        {
            nameof(Basic) => SubscriptionTypeConst.Basic.MaxDailySessions,
            nameof(Plus) => SubscriptionTypeConst.Plus.MaxDailySessions,
            nameof(Premium) => SubscriptionTypeConst.Premium.MaxDailySessions,
            _ => throw new InvalidCastException()
        };        
                
    }
}