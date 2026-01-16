using ErrorOr;
using GymManagement.Application.Members.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Members.Queries.GetMember;

public sealed record GetMemberQuery(Guid MemberId): IRequest<ErrorOr<MemberDto>>;
