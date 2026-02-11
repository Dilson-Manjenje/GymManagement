using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListUpComingSessions;

public record ListUpComingSessionsQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
