using ErrorOr;
using GymManagement.Application.Trainers.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.GetTrainer;

public sealed record GetTrainerQuery(Guid TrainerId): IRequest<ErrorOr<TrainerDto>>;
