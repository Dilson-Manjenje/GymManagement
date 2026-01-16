using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.ListGyms;

public class ListGymsQueryHandler : IRequestHandler<ListGymsQuery, ErrorOr<IEnumerable<GymDto>?>>
{
    private readonly IGymsRepository _gymsRepository;

    public ListGymsQueryHandler(IGymsRepository gymsRepository)
    {
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<IEnumerable<GymDto>?>> Handle(ListGymsQuery query, CancellationToken cancellationToken)
    {
        var gyms = await _gymsRepository.ListAsync();
        var dtos = gyms?.Select(gym => GymDto.MapToDto(gym))
                         .ToList();
        return dtos;
    }
}