using ErrorOr;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.GetSubscription;

public record GetSubscriptionQuery(Guid Id): IRequest<ErrorOr<SubscriptionDto>>;
