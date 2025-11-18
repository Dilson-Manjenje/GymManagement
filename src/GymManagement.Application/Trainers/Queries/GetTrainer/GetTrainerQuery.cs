using ErrorOr;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.GetTrainer;

public sealed record GetTrainerQuery(Guid TrainerId): IRequest<ErrorOr<Trainer>>;
