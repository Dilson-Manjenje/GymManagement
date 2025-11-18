using ErrorOr;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainers;

public record ListTrainersQuery(): IRequest<ErrorOr<IEnumerable<Trainer>?>>;
