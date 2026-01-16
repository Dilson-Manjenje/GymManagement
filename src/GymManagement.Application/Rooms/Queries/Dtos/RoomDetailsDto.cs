using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Rooms;

namespace GymManagement.Application.Rooms.Queries.Dtos;

public record RoomDetailsDto (Guid Id,
                             string Name,
                             int Capacity,
                             bool IsAvailable,
                             Guid GymId,
                             string GymName = ""
                             // ActiveSessions? 
                             )
{
    public static RoomDetailsDto MapToDto(Room room, string gymName)
    {
        return new RoomDetailsDto(Id: room.Id,
                                  Name: room.Name,
                                  Capacity: room.Capacity,
                                  IsAvailable: room.IsAvailable,
                                  GymId: room.GymId,
                                  GymName: gymName);
    }
}                             