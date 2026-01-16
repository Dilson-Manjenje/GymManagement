using ErrorOr;
using GymManagement.Domain.Members;
using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;

namespace GymManagement.Domain.Trainers;

public class Trainer : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    //TODO: Move email from Trainer to Member Entity
    public string? Email { get; private set; } = null;
    public string Specialization { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public Guid GymId { get; private set; }
    public Gym Gym { get; private set; } = null!;
    public Guid MemberId { get; private set; }
    public Member Member { get; private set; } = null!;

    private Trainer() { }
    public Trainer(string name,
                   string phone,
                   string specialization,
                   Guid gymId,
                   Guid memberId,
                   string? email = null,
                   Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Phone = phone;
        Specialization = specialization;
        GymId = gymId;
        MemberId = memberId;
        Email = email;
        IsActive = true;
    }

    public ErrorOr<Success> RemoveTrainer()
    {
        // TODO: Remove if there are is no sessions, otherwise disable
        IsActive = false;
        
        return Result.Success;
    }

    public ErrorOr<Success> Update(string? name = null,
                                 string? phone = null,
                                 string? email = null,
                                 string? specialization = null)
    {
        Name = name ?? Name;
        Phone = phone ?? Phone;
        Email = email ?? Email;
        Specialization = specialization ?? Specialization;

        return Result.Success;
    }
}
