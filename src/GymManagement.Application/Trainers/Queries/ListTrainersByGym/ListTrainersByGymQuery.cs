using ErrorOr;
using GymManagement.Application.Trainers.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainersByGym;

public record ListTrainersByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<TrainerDto>?>>;
