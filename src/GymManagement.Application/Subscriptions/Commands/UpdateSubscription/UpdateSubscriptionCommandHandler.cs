using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSubscriptionCommandHandler(ISubscriptionsRepository subscriptionsRepository,
                                            IUnitOfWork unitOfWork)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Subscription>> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (subscription is null)
            SubscriptionErrors.SubscriptionNotFound(request.Id);

        var result = subscription!.UpdateSubscription(request.SubscriptionType);
        
        if (result.IsError)
            return result.Errors;

        await _subscriptionsRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        
        return subscription;
    }
}