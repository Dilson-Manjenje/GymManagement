using ErrorOr;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Common;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions.Events;
using GymManagement.Domain.Trainers;
using Throw;

namespace GymManagement.Domain.Sessions;

public class Session : Entity
{
    private DateTime _now = DateTime.Now;

    public string Title { get; private set; } = null!;        
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public SessionStatus Status { get; private set; } = SessionStatus.Scheduled;
    public bool IsInProgress => _now >= StartDate && _now < EndDate;    
    public Guid TrainerId { get; private set; }
    public Trainer Trainer { get; set; } = null!;    
    public Guid RoomId { get; private set; }
    public Room Room { get; set; } = null!;    
    public int Capacity { get; private set; }
    
    public int Vacancy { get; private set; }
    // public List<Booking> Bookings { get; set; } = new();

    private Session() { }
    public Session(Guid roomId,
                    Guid trainerId,
                    string title,
                    int capacity,
                    int vacancy,
                    DateTime startDate,
                    DateTime endDate,
                    Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Title = title;
        StartDate = startDate;
        EndDate = endDate;                
        TrainerId = trainerId;
        RoomId = roomId;
        Capacity = capacity;
        Vacancy = vacancy;
    }

    public ErrorOr<Success> UpdateSession(Guid roomId,
                                          Guid trainerId,
                                          string? title,
                                          DateTime? startDate,
                                          DateTime? endDate)
    {
        RoomId = roomId;
        TrainerId = trainerId;
        Title = title ?? Title;
        StartDate = startDate ?? StartDate;
        EndDate = endDate ?? EndDate;

        return Result.Success;
    }

    public ErrorOr<Success> Cancel()
    {
        if (!CanCancelSession())
            return SessionErrors.CantChangeSession(id: Id);

        Status = SessionStatus.Canceled;

        DomainEvents.Add(new SessionCanceledEvent(SessionId: Id));     
        
        return Result.Success;
    }

    public ErrorOr<Success> Finalize()
    {
        if (!CanCancelSession())
            return SessionErrors.CantChangeSession(id: Id);

        // TODO: Add/Raise SessionFinalizedEvent            
        Status = SessionStatus.Finalized;

        return Result.Success;
        
    }

    protected bool CanCancelSession()
    {
        if (SessionStatus.NonCancelableStatus.Contains(Status))
            return false;

        return true;
    }

    public bool IsActive()
    {
        return IsInProgress == true ||
               Status == SessionStatus.Scheduled ||
               Status == SessionStatus.InProgress;
    }
    
    public void DecrementVacancy()
    {
        if (Vacancy > 0)
            Vacancy--;
    }

    public void IncrementVacancy()
    {
        if(Vacancy < Capacity )
            Vacancy++;
    }
}