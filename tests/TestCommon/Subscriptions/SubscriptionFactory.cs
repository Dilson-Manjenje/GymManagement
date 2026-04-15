using System.Reflection.Metadata;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;

namespace TestCommon.Subscriptions;

public static class SubscriptionFactory
{
    public static Subscription CreateSubscription(SubscriptionType? type = null,
                                                  Guid? memberId = null,
                                                  Guid? id = null)
    {
        return new Subscription(
            subscriptionType: type ?? Constants.Subscriptions.DefaultSubscriptionType,
            memberId: memberId ?? Constants.Members.AdminId,
            id: id ?? Constants.Subscriptions.Id
        );
    }
}