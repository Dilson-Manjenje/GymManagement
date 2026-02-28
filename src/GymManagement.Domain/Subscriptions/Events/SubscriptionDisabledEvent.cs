using GymManagement.Domain.Common;

namespace GymManagement.Domain.Subscriptions.Events;

public record SubscriptionDisabledEvent(Guid SubscriptionId): IDomainEvent;