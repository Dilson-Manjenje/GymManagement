using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.GetGym;

public class GetGymQueryHandler : IRequestHandler<GetGymQuery, ErrorOr<GymDto>>
{
    private readonly IGymsRepository _gymsRepository;

    public GetGymQueryHandler(IGymsRepository gymsRepository)
    {
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<GymDto>> Handle(GetGymQuery query, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(query.GymId);

        // TODO: Make repoository return Dto on reading
        return (gym is null)
            ? GymErrors.GymNotFound(query.GymId)
            : GymDto.MapToDto(gym);
    }
}
