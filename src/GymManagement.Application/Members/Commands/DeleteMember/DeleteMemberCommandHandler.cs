using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;


namespace GymManagement.Application.Members.Commands.DeleteMember;

public class  DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, ErrorOr<Unit>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IBookingsRepository _bookingsRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMemberCommandHandler(IUnitOfWork unitOfWork,
                                    IMembersRepository membersRepository,
                                    IBookingsRepository bookingsRepository,
                                    ISubscriptionsRepository subscriptionsRepository)
    {
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
        _bookingsRepository = bookingsRepository;
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMemberCommand command, CancellationToken cancellationToken = default)
    {       
        var member = await _membersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (member is null)
            return MemberErrors.MemberNotFound(command.Id);

        var bookings = await _bookingsRepository.ListByMemberAsync(memberId: command.Id);        
      
        if (bookings is not null && bookings.Any())
            return MemberErrors.CannotRemoveMemberWithBooking(memberId: command.Id);

        var subscriptions = await _subscriptionsRepository.ListByMemberAsync(command.Id);

        if (subscriptions is not null && subscriptions.Any())
            return MemberErrors.CannotDeleteMemberWithSubscription(memberId: command.Id);
            
        await _membersRepository.RemoveAsync(member, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}