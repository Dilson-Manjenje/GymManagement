using ErrorOr;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.GetAllSubscriptions;

public record GetAllSubscriptionsQuery(): IRequest<ErrorOr<IEnumerable<Subscription>?>>;
