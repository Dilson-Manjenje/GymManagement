using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Members;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Subscriptions;
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
        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);
        if (session is null)
            return SessionErrors.SessionNotFound(command.SessionId);

        var member = await _membersRepository.GetByIdAsync(command.MemberId);
        if (member is null)
            return MemberErrors.MemberNotFound(command.MemberId);
       
        if (member.GymId is null || member.GymId == Guid.Empty)
            return MemberErrors.MemberDontHaveGym(member.Id);
        
        var existingBooking = await _bookingsRepository.GetByMemberAndSessionAsync(member.Id, session.Id, cancellationToken);
        if (existingBooking is not null)
            return BookingErrors.DuplicateBooking(member.Id, session.Id);

        var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionAsync(memberId: member.Id);
        if (activeSubscription is null)
            return BookingErrors.MemberDontHaveActiveSubscription(member.Id);

        var bookingResult = Booking.Create(member, session, activeSubscription);

        if (bookingResult.IsError)
            return bookingResult.Errors;

        var booking = bookingResult.Value;   

        // TODO: Add try/catch to handle ConcurrencyException and RaceCondition on save Booking
        await _bookingsRepository.AddAsync(booking);        
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        // Session Vacancy is decremented in Eventual Consistency manner by BookingCreatedEvent

        return booking.Id;
    }
}