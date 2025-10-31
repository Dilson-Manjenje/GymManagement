using ErrorOr;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptions;

public record ListSubscriptionsQuery(): IRequest<ErrorOr<IEnumerable<Subscription>?>>;
