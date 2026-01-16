using ErrorOr;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByGym;

public record ListSubscriptionsByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<SubscriptionDto>?>>;
