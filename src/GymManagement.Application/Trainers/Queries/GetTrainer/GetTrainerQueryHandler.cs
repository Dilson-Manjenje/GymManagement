using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Trainers.Queries.Dtos;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Queries.GetTrainer;

public class GetTrainerQueryHandler : IRequestHandler<GetTrainerQuery, ErrorOr<TrainerDto>>
{
    private readonly ITrainersRepository _trainerRepository;

    public GetTrainerQueryHandler(ITrainersRepository trainerRepository)
    {
        _trainerRepository = trainerRepository;
    }

    public async Task<ErrorOr<TrainerDto>> Handle(GetTrainerQuery query, CancellationToken cancellationToken)
    {
        var trainer = await _trainerRepository.GetByIdAsync(query.TrainerId);

        return (trainer is null)
            ? TrainerErrors.TrainerNotFound(query.TrainerId)
            : TrainerDto.MapToDto(trainer);
    }
}