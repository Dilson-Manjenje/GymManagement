using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Gyms;

namespace GymManagement.Application.Gyms.Queries.Dtos;

public record GymDto(Guid Id, string Name, string Address)
{
    public static GymDto MapToDto(Gym gym)
    {
        return new GymDto(Id: gym.Id,
                          Name: gym.Name,
                          Address: gym.Address);
    }
}