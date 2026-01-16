using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Trainers.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainersByGym;

public class ListTrainersByGymQueryHandler : IRequestHandler<ListTrainersByGymQuery, ErrorOr<IEnumerable<TrainerDto>?>>
{
    private readonly ITrainersRepository _trainersRepository;

    public ListTrainersByGymQueryHandler(ITrainersRepository trainersRepository)
    {
        _trainersRepository = trainersRepository;
    }

    public async Task<ErrorOr<IEnumerable<TrainerDto>?>> Handle(ListTrainersByGymQuery query, CancellationToken cancellationToken)
    {
        var trainers = await _trainersRepository.ListByGymIdAsync(query.GymId);
        return trainers?.Select(t => TrainerDto.MapToDto(t)).ToList();
    }
}