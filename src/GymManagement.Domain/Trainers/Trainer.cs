using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Admins;
using GymManagement.Domain.Gyms;

namespace GymManagement.Domain.Trainers;

public class Trainer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; } = null!;
    public string Specialization { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public Guid GymId { get; private set; }
    public Gym Gym { get; private set; } = null!;
    public Guid AdminId { get; private set; }
    public Admin Admin { get; private set; } = null!;

    private Trainer() { }
    public Trainer(string name,
                   string phone,
                   string specialization,
                   Guid gymId,
                   Guid adminId,
                   string? email = null,
                   Guid? id = null)
    {
        Name = name;
        Phone = phone;
        Specialization = specialization;
        GymId = gymId;
        AdminId = adminId;
        Email = email;
        IsActive = true;
        Id = id ?? Guid.NewGuid();
    }

    public ErrorOr<Success> RemoveTrainer()
    {
        // TODO: Remove if there are is no sessions, otherwise disable
        IsActive = false;
        
        return Result.Success;
    }
}
