using ErrorOr;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByMember;

public record ListSubscriptionsByMemberQuery(Guid MemberId): IRequest<ErrorOr<IEnumerable<SubscriptionDto>?>>;
