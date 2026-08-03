using ErrorOr;
using GymManagement.Domain.Members;
using GymManagement.Domain.Common;
using GymManagement.Domain.Rooms;


namespace GymManagement.Domain.Gyms;

public class Gym : Entity
{
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    public Gym(
        string name,
        string address,        
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Address = address;        
    }

    private Gym() { }

    public ErrorOr<Success> UpdateGym(string? name = null, string? address = null)
    {
        Name = name ?? Name;
        Address = address ?? Address;

        return Result.Success;
    } 
}