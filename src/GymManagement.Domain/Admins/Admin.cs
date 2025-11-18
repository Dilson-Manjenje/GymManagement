using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Trainers;
using Throw;

namespace GymManagement.Domain.Admins;

public class Admin
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; } = null;
    public string UserName { get; set; } = string.Empty;
    public Guid? SubscriptionId { get; private set; } = null;
    public Trainer? Trainer { get; set; } = null; // Navigation for 1:1

    public Admin(
        string userName,
        Guid? userId = null,
        Guid? subscriptionId = null,
        Guid? id = null)
    {
        UserName = userName;
        UserId = userId ?? Guid.NewGuid();
        SubscriptionId = subscriptionId;
        Id = id ?? Guid.NewGuid();
    }

    private Admin() { }

    public void SetSubscription(Subscription subscription)
    {
        SubscriptionId.HasValue.Throw().IfTrue();

        SubscriptionId = subscription.Id;
    }

    public void DeleteSubscription(Guid subscriptionId)
    {
        SubscriptionId.ThrowIfNull().IfNotEquals(subscriptionId);

        SubscriptionId = null;
    }
}