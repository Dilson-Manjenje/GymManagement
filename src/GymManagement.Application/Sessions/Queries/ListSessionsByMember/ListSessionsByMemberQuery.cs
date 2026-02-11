using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByMember;

public record ListSessionsByMemberQuery(Guid MemberId): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
