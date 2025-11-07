using GymManagement.Application.Rooms.Commands.Shared;

namespace GymManagement.Application.Rooms.Commands.CreateRoom;

public record CreateRoomCommand(string Name,
                                int Capacity,
                                Guid GymId) : RoomBaseCommand(Name, Capacity, GymId);