namespace GymManagement.Contracts.Rooms;

public record CreateUpdateRoomRequest(    
    string Name,
    int Capacity,
    Guid GymId);
