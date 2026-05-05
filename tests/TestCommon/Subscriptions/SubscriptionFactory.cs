using System.Reflection.Metadata;
using GymManagement.Domain.Subscriptions;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace TestCommon.Subscriptions;

public static class SubscriptionFactory
{
    public static Subscription CreateSubscription(SubscriptionType? type = null,
                                                  Guid? memberId = null,
                                                  Guid? id = null)
    {
        var subs = new Subscription(
            subscriptionType: type ?? Constants.Subscriptions.DefaultSubscriptionType,
            memberId: memberId ?? Constants.Members.AdminId,
            id: id ?? Constants.Subscriptions.NewId
        );
        
        return subs;
    }
}