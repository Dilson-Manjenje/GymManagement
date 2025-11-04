using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.ListGyms;

public class ListGymsQueryHandler : IRequestHandler<ListGymsQuery, ErrorOr<IEnumerable<Gym>?>>
{
    private readonly IGymsRepository _gymsRepository;

    public ListGymsQueryHandler(IGymsRepository gymsRepository)
    {
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<IEnumerable<Gym>?>> Handle(ListGymsQuery request, CancellationToken cancellationToken)
    {
        var gyms = await _gymsRepository.ListAsync();

        return gyms?.ToList();
    }
}