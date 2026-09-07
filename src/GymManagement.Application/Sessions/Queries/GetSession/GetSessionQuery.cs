using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.GetSession;

public record GetSessionQuery(Guid Id): IRequest<ErrorOr<SessionDto>>;
