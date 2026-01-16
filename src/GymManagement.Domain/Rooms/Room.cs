using ErrorOr;
using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;

namespace GymManagement.Domain.Rooms;

public class Room : Entity
{
    public string Name { get; private set; } = null!;
    public int Capacity { get; private set; }
    public Guid GymId { get; private set; }
    public Gym Gym { get; set; } = null!;
    public bool IsAvailable { get; private set; } = true;
    public Room(
        string name,
        int capacity,
        Guid gymId,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Capacity = capacity;
        GymId = gymId;
    }
    private Room() { }
    // TODO: Add RoomType, Equipment
    public ErrorOr<Success> UpdateRoom(string? newName = null,
                                       int? newCapacity = null,
                                       Guid? gymId = null)
    {
        Name = newName ?? Name;
        Capacity = newCapacity ?? Capacity;
        GymId = gymId ?? GymId;

        return Result.Success;
    }

    public ErrorOr<Success> DisableRoom()
    {
        IsAvailable = false;
        
        return Result.Success;
    }
}