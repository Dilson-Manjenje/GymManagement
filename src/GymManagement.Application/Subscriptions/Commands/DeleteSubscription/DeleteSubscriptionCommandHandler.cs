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

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, ErrorOr<Unit>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    public readonly IBookingsRepository _bookingsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IMembersRepository membersRepository,
                                            IBookingsRepository bookingsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            return SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId);

        if (subscription.IsActive)
            return SubscriptionErrors.CantDeleteActiveSubscription();

        var bookings = await _bookingsRepository.ListByMemberAsync(subscription.MemberId);

        if (bookings is not null && bookings.Any())
            return SubscriptionErrors.CantDeleteSubscriptionWithBookings(subscription.Id);
        
        await _subscriptionsRepository.RemoveAsync(subscription, cancellationToken); 
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        

        return Unit.Value;
    }
}
    