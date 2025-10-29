using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, ErrorOr<IEnumerable<Subscription>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public GetAllSubscriptionsQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }
    
    public async Task<ErrorOr<IEnumerable<Subscription>?>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken = default)
    {
        var subscriptions = await _subscriptionsRepository.GetAllAsync();
        
        return subscriptions?.ToList();       
    }
}