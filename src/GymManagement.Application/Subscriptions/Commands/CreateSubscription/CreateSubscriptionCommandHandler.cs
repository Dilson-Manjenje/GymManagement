using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateSubscriptionCommandHandler(ISubscriptionsRepository subscriptionsRepository,
                                            IUnitOfWork unitOfWork)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: Check if Admin/User exists, validate SubscriptionType, etc.
        var subscription = new Subscription(subscriptionType: command.SubscriptionType,
                                            adminId: command.AdminId);   

        await _subscriptionsRepository.AddSubscriptionAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        
        return subscription;
    }
}