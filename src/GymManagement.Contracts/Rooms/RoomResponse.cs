namespace GymManagement.Contracts.Rooms;

public record RoomResponse(
    Guid Id,
    string Name,
    int Capacity,
    bool IsAvailable,
    Guid GymId,
    string GymName
    );