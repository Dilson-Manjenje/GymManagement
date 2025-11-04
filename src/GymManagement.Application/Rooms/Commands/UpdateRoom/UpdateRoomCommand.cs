using GymManagement.Application.Rooms.Commands.Shared;

namespace GymManagement.Application.Rooms.Commands.UpdateRoom;
public record UpdateRoomCommand(Guid Id,
                                string Name,
                                int Capacity,
                                Guid GymId) : RoomBaseCommand(Name, Capacity, GymId);
