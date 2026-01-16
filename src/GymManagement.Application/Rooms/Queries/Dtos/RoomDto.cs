using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Rooms;

namespace GymManagement.Application.Rooms.Queries.Dtos;

public record RoomDto(Guid Id,
                             string Name,
                             int Capacity,
                             bool IsAvailable,
                             Guid GymId,
                             string GymName
                             // ActiveSessions? 
                             // IReadOnlyList<SessionDetailsDto>? Sessions = null
                             )
{
    public static RoomDto MapToDto(Room room)
    {
        return new RoomDto(Id: room.Id,
                                  Name: room.Name,
                                  Capacity: room.Capacity,
                                  IsAvailable: room.IsAvailable,
                                  GymId: room.GymId,
                                  GymName: room.Gym.Name);
    }
}                             