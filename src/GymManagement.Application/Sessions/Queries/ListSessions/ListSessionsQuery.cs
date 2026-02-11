using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessions;

public record ListSessionsQuery(): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
