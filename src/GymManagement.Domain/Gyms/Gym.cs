using ErrorOr;
using GymManagement.Domain.Rooms;
using Throw;

namespace GymManagement.Domain.Gyms;

public class Gym
{
    private readonly int _maxRooms;
    public Guid Id { get; }
    //TODO: Tranform roomsIds into ICollection<Room>
    //private readonly List<Guid> _roomIds = new(); 
    public List<Room> Rooms { get; private set; } = new();
    //private readonly List<Guid> _trainerIds = new();
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

    void RemoveRoom(Guid roomId)
    {
        //_roomIds.Remove(roomId);
        var room = Rooms.FirstOrDefault( r => r.Id == roomId);
        if (room is not null)
            Rooms.Remove(room);
        
    }    

    // TODO: Move AddTrainer and HasTrainer to a Room entity
    // public ErrorOr<Success> AddTrainer(Guid trainerId)
    // {
    //     if (_trainerIds.Contains(trainerId))
    //     {
    //         return Error.Conflict(description: "Trainer already added to gym");
    //     }

    //     _trainerIds.Add(trainerId);
    //     return Result.Success;
    // }

    // public bool HasTrainer(Guid trainerId)
    // {
    //     return _trainerIds.Contains(trainerId);
    // }   
}