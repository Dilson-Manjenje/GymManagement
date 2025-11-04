namespace GymManagement.Contracts.Rooms;

public record ListRoomsResponse(IEnumerable<RoomResponse> Rooms);