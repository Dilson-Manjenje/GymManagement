using ErrorOr;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptions;

public record ListSubscriptionsQuery(): IRequest<ErrorOr<IEnumerable<SubscriptionDto>?>>;
