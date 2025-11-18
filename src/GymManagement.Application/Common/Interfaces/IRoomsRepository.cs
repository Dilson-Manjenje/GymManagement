using GymManagement.Domain.Rooms;

namespace GymManagement.Application.Common.Interfaces;

public interface IRoomsRepository
{
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    //Task<Room?> GetByName(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>?> GetRoomsByGymIdAsync(Guid gymId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);     
    Task<IEnumerable<Room>?> ListAsync(CancellationToken cancellationToken = default);    
}