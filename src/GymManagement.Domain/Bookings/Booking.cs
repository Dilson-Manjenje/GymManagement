using ErrorOr;
using GymManagement.Domain.Common;
using GymManagement.Domain.Members;
using GymManagement.Domain.Sessions;

namespace GymManagement.Domain.Bookings;

public class Booking : Entity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; set; } = null!;        
    public Guid SessionId { get; private set; }
    public Session Session { get; set; } = null!;
    public BookingStatus Status { get; private set; } = BookingStatus.Active;
    
    private Booking() {}
    public Booking(Guid sessionId,
                   Guid memberId, Guid? id = null) : base(id ?? Guid.NewGuid())
    {

        SessionId = sessionId;
        MemberId = memberId;
        Status = BookingStatus.Active;

        //Add/Raise Event BookingCreatedEvent => Increase Session Vacancy if active
    }
    

    public ErrorOr<Success> Cancel()
    {
        if (BookingStatus.NonCancelableStatus.Contains(Status))
            return BookingErrors.CantChangeBooking(id: Id, statusName: Status.Name);

        Status = BookingStatus.Canceled;
        
        //Raise BookingCanceledEvent => Decrease Session Vacancy if active        
        return Result.Success;
    }    
    
    public ErrorOr<Success> Finalize()
    {
        if (BookingStatus.NonCancelableStatus.Contains(Status))
            return BookingErrors.CantChangeBooking(id: Id, statusName: Status.Name);
           
        Status = BookingStatus.Finalized;

        return Result.Success;
    }    

}