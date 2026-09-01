using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;

namespace TestCommon.Subscriptions;

public static class SubscriptionCommandFactory
{
    public static CreateSubscriptionCommand GetCreateSubscriptionCommand(SubscriptionType? type = null,
                                                  Guid? memberId = null)
    {
        return new CreateSubscriptionCommand(
            SubscriptionType: type ?? Constants.Subscriptions.DefaultSubscriptionType,
            MemberId: memberId ?? Constants.Members.AdminId);                
    }
}