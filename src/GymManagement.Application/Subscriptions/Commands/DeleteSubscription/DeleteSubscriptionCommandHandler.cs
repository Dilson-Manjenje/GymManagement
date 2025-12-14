using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, ErrorOr<Deleted>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IUnitOfWork _unitOfWork;
                                           
    public DeleteSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IMembersRepository membersRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            return SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId);

        var member = await _membersRepository.GetByIdAsync(subscription.MemberId, cancellationToken);
        if (member is null)
            return MemberErrors.UserNotFound(subscription.MemberId);

        // TODO: Check if can allow remove active subscription?        
        await _subscriptionsRepository.RemoveAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
    