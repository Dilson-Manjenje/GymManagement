using ErrorOr;
using GymManagement.Domain.Admins;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Trainers;
using Throw;

namespace GymManagement.Domain.Gyms;

public class Gym
{
    private readonly int _maxRooms;
    public Guid Id { get; }    
    public List<Room> Rooms { get; private set; } = new();
    public List<Trainer> Trainers { get; private set; } = new();    
    public List<Admin> Admins { get; private set; } = new();    
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    public Gym(
        string name,
        string address,
        int maxRooms = 100,
        Guid? id = null)
    {
        Name = name;
        Address = address;
        _maxRooms = maxRooms;
        Id = id ?? Guid.NewGuid();
        //_roomIds = Rooms?.Select( r => r.Id).ToList() ?? new List<Guid>();
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
        //_roomIds.Throw().IfContains(room.Id);
        //var _roomIds = Rooms.Select( r => r.Id).Throw().IfContains(room.Id);
        
        var exist = Rooms.Select( r => r.Id).Contains(room.Id);
        if (exist)
            return GymErrors.RoomAlreadyAddedToGym(room.Id);
        
        if (Rooms.Count >= _maxRooms)
            return GymErrors.CantExceedMaxRooms;
        
        //_roomIds.Add(room.Id);
        Rooms.Add(room);

        return Result.Success;
    }

    public bool HasRoom(Guid roomId)
    {
        //return _roomIds.Contains(roomId);
        return Rooms.Select(r => r.Id).Contains(roomId);        
    }

    public ErrorOr<Success> RemoveRoom(Guid roomId)
    {
        //_roomIds.Remove(roomId);
        var room = Rooms.FirstOrDefault( r => r.Id == roomId);
        if (room is not null)
            Rooms.Remove(room);
        
        return Result.Success;
    }    

    public ErrorOr<Success> AddTrainer(Trainer trainer)
    {
        var exist = Trainers.Select( t => t.Id).Contains(trainer.Id);
        if (exist)
            return TrainerErrors.TrainerAlreadyAddedToGym(trainer.Id);

        Trainers.Add(trainer);

        return Result.Success;
    }

    public bool HasTrainer(Guid trainerId)
    {
        return Trainers.Select(t => t.Id).Contains(trainerId);
    }   
    
    public ErrorOr<Success> RemoveTrainer(Guid trainerId)
    {
        var trainer = Trainers.FirstOrDefault(t => t.Id == trainerId);
        
        if (trainer is null)
            return GymErrors.TrainerNotAssociated(trainerId, Id);

        // TODO: Check if trainer has session 
        // if (TrainerHasSession(trainerId))
        //     return GymErrors.CannotRemoveTrainerWithScheduledSessions(trainerId);
        
        Trainers.Remove(trainer);
        
        return Result.Success;
    }  
}