namespace GymManagement.Contracts.Rooms;

public record RoomRequest(string Name,
                          int Capacity,
                          Guid GymId);
