using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.ListTrainers;

public class ListTrainersQueryHandler : IRequestHandler<ListTrainersQuery, ErrorOr<IEnumerable<Trainer>?>>
{
    private readonly ITrainersRepository _trainersRepository;

    public ListTrainersQueryHandler(ITrainersRepository trainersRepository)
    {
        _trainersRepository = trainersRepository;
    }

    public async Task<ErrorOr<IEnumerable<Trainer>?>> Handle(ListTrainersQuery query, CancellationToken cancellationToken)
    {
        var trainers = await _trainersRepository.ListAsync();

        return trainers?.ToList();
    }
}