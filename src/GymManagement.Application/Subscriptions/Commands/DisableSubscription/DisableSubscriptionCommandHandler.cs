using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.DisableSubscription;

public class DisableSubscriptionCommandHandler : IRequestHandler<DisableSubscriptionCommand, ErrorOr<Guid>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IUnitOfWork _unitOfWork;
                                           
    public DisableSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IMembersRepository membersRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(DisableSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            return SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId);

        if (!subscription.IsActive)
            return SubscriptionErrors.CantChangeExpiredSubscription();
            
        var disabled = subscription.DisableSubscription();
        if (disabled.IsError)
            return disabled.Errors;

        await _subscriptionsRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return subscription.Id;
    }
}
    