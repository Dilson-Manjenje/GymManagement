using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, ErrorOr<Deleted>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSubscriptionCommandHandler(ISubscriptionsRepository subscriptionsRepository,
                                          IUnitOfWork unitOfWork)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(request.Id, cancellationToken);
        if (subscription is null)
            return Error.NotFound(code: "Subscription.NotFound",
                                  description: $"Subscription with ID '{request.Id}' not found.");
        
        // TODO: check if user exist, remove/cancel subscription from user/admins, 

        await _subscriptionsRepository.RemoveSubscription(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
    