using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByGym;

public record ListSessionsByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
