using ErrorOr;
using GymManagement.Domain.Bookings.Events;
using GymManagement.Domain.Common;
using GymManagement.Domain.Members;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Domain.Bookings;

public class Booking : Entity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; set; } = null!;        
    public Guid SessionId { get; private set; }
    public Session Session { get; set; } = null!;
    public BookingStatus Status { get; private set; } = BookingStatus.Active;
    
    private Booking() {}
    private Booking(Guid sessionId,
                   Guid memberId, Guid? id = null) : base(id ?? Guid.NewGuid())
    {

        SessionId = sessionId;
        MemberId = memberId;
        Status = BookingStatus.Active;

        _domainEvents.Add(new BookingCreatedEvent(BookingId: Id, SessionId: sessionId));
    }

    public static ErrorOr<Booking> Create(Member member, Session session, Subscription activeSubscription)
    {
        if (session.Vacancy == 0)
            return SessionErrors.CannotExceedSessionCapacity;

        if (session.Room.GymId != member.GymId)
            return BookingErrors.MemberNotInTheSameGym(memberId: member.Id);

        if (!SessionStatus.ActiveStatus.Contains(session.Status))
            return BookingErrors.InvalidSessionsStatus(id: session.Id, statusName: session.Status.Name);

        if (!activeSubscription.HasRoom(session.RoomId))
            return BookingErrors.SubscriptionDontHaveAccess(subscriptionId: activeSubscription.Id, roomId: session.RoomId);
        
        return new Booking(sessionId: session.Id, memberId: member.Id);      
    }

    public ErrorOr<Success> Cancel()
    {
        if (BookingStatus.NonCancelableStatus.Contains(Status))
            return BookingErrors.CantChangeBooking(id: Id, statusName: Status.Name);

        Status = BookingStatus.Canceled;
        
        _domainEvents.Add(new BookingCanceledEvent(BookingId: Id, SessionId: SessionId));    

        return Result.Success;
    }    
    
    public ErrorOr<Success> Finalize()
    {
        if (BookingStatus.NonCancelableStatus.Contains(Status))
            return BookingErrors.CantChangeBooking(id: Id, statusName: Status.Name);

        Status = BookingStatus.Finalized;

        _domainEvents.Add(new BookingFinalizedEvent(BookingId: Id, SessionId: SessionId)); 
        return Result.Success;
    }    

}