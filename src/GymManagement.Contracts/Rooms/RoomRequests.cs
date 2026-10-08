namespace GymManagement.Contracts.Rooms;

public record CreateRoomRequest(string Name,
                          int Capacity,
                          Guid GymId);

public record UpdateRoomRequest(Guid Id,
                                string Name,
                                int Capacity);
