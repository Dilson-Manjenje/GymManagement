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

    public bool HasTrainer(Trainer trainer)
    {
        var exist = Trainers.Any(t => t.Id.Equals(trainer.Id)) ||
                    Trainers.Any(t => t.MemberId == trainer.MemberId);
        
        return exist;
    }
}