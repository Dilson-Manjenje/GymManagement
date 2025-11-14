using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptions;

public class ListSubscriptionsQueryHandler : IRequestHandler<ListSubscriptionsQuery, ErrorOr<IEnumerable<Subscription>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListSubscriptionsQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }
    
    public async Task<ErrorOr<IEnumerable<Subscription>?>> Handle(ListSubscriptionsQuery query, CancellationToken cancellationToken = default)
    {
        var subscriptions = await _subscriptionsRepository.ListAsync();
        
        return subscriptions?.ToList();       
    }
}