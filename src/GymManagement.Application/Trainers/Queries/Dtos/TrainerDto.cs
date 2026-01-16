using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Members;
using GymManagement.Domain.Trainers;

namespace GymManagement.Application.Trainers.Queries.Dtos;

public record TrainerDto(Guid Id,
                         string Name,
                         string Phone,
                         string? Email,
                         string Specialization,
                         bool IsActive,
                         Guid MemberId,
                         Guid GymId,
                         string GymName)
{
    public static TrainerDto MapToDto(Trainer trainer)
    {
        return new TrainerDto(Id: trainer.Id,
                              Name: trainer.Name,
                              Phone: trainer.Phone,
                              Email: trainer.Email,
                              Specialization: trainer.Specialization,
                              IsActive: trainer.IsActive,
                              MemberId: trainer.MemberId,
                              GymId: trainer.GymId,
                              GymName: trainer.Gym.Name);
    }
}