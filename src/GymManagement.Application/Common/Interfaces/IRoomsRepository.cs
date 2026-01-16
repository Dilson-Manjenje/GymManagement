using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Domain.Rooms;

namespace GymManagement.Application.Common.Interfaces;

public interface IRoomsRepository
{
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoomDto?> GetWithDetails(Guid roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>?> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>?> ListByGymAsync(Guid gymId, CancellationToken cancellationToken = default);
    Task<bool> RoomHasOverlappingSession(Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken = default);

}