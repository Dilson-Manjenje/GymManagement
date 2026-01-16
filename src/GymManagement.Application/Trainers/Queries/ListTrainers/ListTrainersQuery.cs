using ErrorOr;
using GymManagement.Application.Trainers.Queries.Dtos;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainers;

public record ListTrainersQuery(): IRequest<ErrorOr<IEnumerable<TrainerDto>?>>;
