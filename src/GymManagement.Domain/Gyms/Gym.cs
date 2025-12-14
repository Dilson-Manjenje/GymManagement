using ErrorOr;
using GymManagement.Domain.Members;
using GymManagement.Domain.Common;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Trainers;
using Throw;

namespace GymManagement.Domain.Gyms;

public class Gym : Entity
{
    private readonly int _maxRooms;
    public List<Room> Rooms { get; private set; } = new();
    public List<Trainer> Trainers { get; private set; } = new();    
    public List<Member> Members { get; private set; } = new();    
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    public Gym(
        string name,
        string address,
        int maxRooms = 100,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Address = address;
        _maxRooms = maxRooms;
    }

    private Gym() { }

    public ErrorOr<Success> UpdateGym(string name, string address)
    {
        Name = name;
        Address = address;

        return Result.Success;
    } 
    public ErrorOr<Success> AddRoom(Room room)
    {
        var exist = Rooms.Select( r => r.Id).Contains(room.Id);
        if (exist)
            return GymErrors.RoomAlreadyAddedToGym(room.Id);

        if (Rooms.Count >= _maxRooms)
            return GymErrors.CantExceedMaxRooms;   
               
        Rooms.Add(room);

        return Result.Success;
    }

    public bool HasRoom(Guid roomId)
    {
        return Rooms.Select(r => r.Id).Contains(roomId);        
    }

    public ErrorOr<Success> RemoveRoom(Guid roomId)
    {
        var room = Rooms.FirstOrDefault( r => r.Id == roomId);
        if (room is not null)
            Rooms.Remove(room);
        
        return Result.Success;
    }    

    public ErrorOr<Success> AddTrainer(Trainer trainer)
    {
        var exist = Trainers.Any(t => t.Id.Equals(trainer.Id)) ||
                    Trainers.Any(t => t.MemberId == trainer.MemberId);
        if (exist)
            return TrainerErrors.TrainerAlreadyAddedToGym(memberId: trainer.MemberId);

        Trainers.Add(trainer);

        return Result.Success;
    }

    public bool HasTrainer(Guid memberId)
    {
        return Trainers.Any(t => t.MemberId == memberId);                
    }   
    
    public ErrorOr<Success> RemoveTrainer(Guid trainerId)
    {
        var trainer = Trainers.FirstOrDefault(t => t.Id == trainerId);
        
        if (trainer is null)
            return GymErrors.TrainerNotAssociated(trainerId, Id);

        // TODO: Check if trainer has booking/sessions
        // if (TrainerHasSession(trainerId))
        //     return GymErrors.CannotRemoveTrainerWithScheduledSessions(trainerId);
        
        Trainers.Remove(trainer);
        
        return Result.Success;
    }  
}