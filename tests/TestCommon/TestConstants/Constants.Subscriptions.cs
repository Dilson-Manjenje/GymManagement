using GymManagement.Domain.Subscriptions;

namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Subscriptions
    {
        public static readonly SubscriptionType DefaultSubscriptionType = SubscriptionType.Basic;
        public static readonly Guid Id = Guid.NewGuid();
        public const int BasicMaxDailySessions = SubscriptionTypeConst.Basic.MaxDailySessions;
        public const int BasicMaxRooms = SubscriptionTypeConst.Basic.MaxRooms;

    }
}