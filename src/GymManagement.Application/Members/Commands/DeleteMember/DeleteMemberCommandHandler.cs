using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;


namespace GymManagement.Application.Members.Commands.DeleteMember;

public class  DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, ErrorOr<Unit>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IBookingsRepository _bookingRepository;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMemberCommandHandler(IUnitOfWork unitOfWork,
                                    IMembersRepository membersRepository,
                                    IBookingsRepository bookingRepository,
                                    ISessionsRepository sessionsRepository)
    {
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
        _bookingRepository = bookingRepository;
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMemberCommand command, CancellationToken cancellationToken = default)
    {       
        var member = await _membersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (member is null)
            return MemberErrors.MemberNotFound(command.Id);

        var bookings = await _bookingRepository.ListByMemberAsync(memberId: command.Id);        
      
        if (bookings is not null && bookings.Any())
            return MemberErrors.CannotRemoveMemberWithBookingSessions(memberId: command.Id);

        var sessions = await _sessionsRepository.ListByMember(command.Id);

        if (sessions is not null && sessions.Any())
            return MemberErrors.CannotRemoveMemberWithBookingSessions(memberId: command.Id);
            
        await _membersRepository.RemoveAsync(member, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}