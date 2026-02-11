using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByTrainer;

public record ListSessionsByTrainerQuery(Guid TrainerId): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
