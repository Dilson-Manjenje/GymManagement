using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Trainers.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainers;

public class ListTrainersQueryHandler : IRequestHandler<ListTrainersQuery, ErrorOr<IEnumerable<TrainerDto>?>>
{
    private readonly ITrainersRepository _trainersRepository;

    public ListTrainersQueryHandler(ITrainersRepository trainersRepository)
    {
        _trainersRepository = trainersRepository;
    }

    public async Task<ErrorOr<IEnumerable<TrainerDto>?>> Handle(ListTrainersQuery query, CancellationToken cancellationToken)
    {
        var trainers = await _trainersRepository.ListAsync();
        return trainers?.Select(t => TrainerDto.MapToDto(t)).ToList();
    }
}