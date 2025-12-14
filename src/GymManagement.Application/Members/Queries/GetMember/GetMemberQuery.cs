using ErrorOr;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Queries.GetMember;

public sealed record GetMemberQuery(Guid MemberId): IRequest<ErrorOr<Member>>;
