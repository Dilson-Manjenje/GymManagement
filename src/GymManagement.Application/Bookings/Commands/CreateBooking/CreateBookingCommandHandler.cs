using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Members;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ErrorOr<Guid>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly ISubscriptionsRepository _subscriptionRepository;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(IMembersRepository membersRepository,
                                       ISubscriptionsRepository subscriptionRepository,
                                       ISessionsRepository sessionsRepository,
                                       IBookingsRepository bookingsRepository,
                                       IUnitOfWork unitOfWork)
    {
        _membersRepository = membersRepository;
        _subscriptionRepository = subscriptionRepository;
        _sessionsRepository = sessionsRepository;
        _bookingsRepository = bookingsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetByIdAsync(command.MemberId);

        if (member is null)
            return MemberErrors.MemberNotFound(command.MemberId);

        if (member.GymId is null || member.GymId == Guid.Empty)
            return MemberErrors.MemberDontHaveGym(command.MemberId);

        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);

        if (session is null)
            return SessionErrors.SessionNotFound(command.SessionId);

        if (session.Room.GymId != member.GymId)
            return BookingErrors.MemberNotInTheSameGym(memberId: member.Id);

        if (!SessionStatus.ActiveStatus.Contains(session.Status))
            return BookingErrors.InvalidSessionsStatus(id: session.Id, statusName: session.Status.Name);
        
        // var hasActiveSubscription = await _subscriptionRepository.HasActiveSubscription(member.Id);
        // if (!hasActiveSubscription)
        //     return BookingErrors.MemberDontHaveActiveSubscription(member.Id);

        var subscription = await _subscriptionRepository.GetActiveSubscriptionAsync(memberId: member.Id);
        if (subscription is null)
            return BookingErrors.MemberDontHaveActiveSubscription(member.Id);

        var hasRoom = subscription.SubscriptionRooms.Any(sr => sr.RoomId == session.RoomId);
        if (!hasRoom)
            return BookingErrors.SubscriptionDontHaveAccess(subscriptionId: subscription.Id, roomId: session.RoomId);

        var existingBooking = await _bookingsRepository
                                            .GetByMemberAndSessionAsync(member.Id, session.Id, cancellationToken);

        if (existingBooking is not null)
            return BookingErrors.DuplicateBooking(member.Id, session.Id);
        
        if (session.Vacancy == 0)
            return SessionErrors.CannotExceedSessionCapacity;

        var booking = new Booking(sessionId: session.Id, memberId: member.Id);

        session.DecrementVacancy();

        // TODO: Add try/catch to handle ConcurrencyException and RaceCondition on save Booking
        await _bookingsRepository.AddAsync(booking);
        
        await _sessionsRepository.UpdateAsync(session); 
        await _unitOfWork.CommitChangesAsync();

        return booking.Id;
    }

    private async Task<ErrorOr<Success>> ValidateBookingRules(Member member,
                                                                     Session session,
                                                                     CancellationToken cancellationToken = default)
    {
        if (member.GymId is null || member.GymId == Guid.Empty)
            return MemberErrors.MemberDontHaveGym(member.Id);

        var hasActiveSubscription = await _subscriptionRepository.HasActiveSubscription(member.Id);
        if (!hasActiveSubscription)
            BookingErrors.MemberDontHaveActiveSubscription(member.Id);

        if (session.Room.GymId != member.GymId)
            return BookingErrors.MemberNotInTheSameGym(member.Id);

        if (!SessionStatus.ActiveStatus.Contains(session.Status))
            return BookingErrors.InvalidSessionsStatus(session.Id, session.Status.Name);

        if (session.Vacancy == 0)
            return SessionErrors.CannotExceedSessionCapacity;

        var existingBooking = await _bookingsRepository.GetByMemberAndSessionAsync(member.Id, session.Id, cancellationToken);

        if (existingBooking is not null && existingBooking.Status == BookingStatus.Active)
            return BookingErrors.DuplicateBooking(member.Id, session.Id);
        
        return Result.Success;
    }
}