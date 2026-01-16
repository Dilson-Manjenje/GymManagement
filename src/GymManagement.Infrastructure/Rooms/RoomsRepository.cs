using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Domain.Rooms;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Rooms.Persistence;

internal class RoomsRepository : IRoomsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public RoomsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    async Task IRoomsRepository.AddAsync(Room room, CancellationToken cancellationToken)
    {
        await _dbContext.Rooms.AddAsync(room);
    }
    
    async Task IRoomsRepository.UpdateAsync(Room room, CancellationToken cancellationToken)
    {
        _dbContext.Rooms.Update(room);
        await Task.CompletedTask;                       
    }

    async Task<Room?> IRoomsRepository.GetByIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        //return await _dbContext.Rooms.FindAsync(roomId, cancellationToken);
        return await _dbContext.Rooms
                        .Where(x => x.Id == roomId)
                        .Include(x => x.Gym) // TODO: Refactor to use projection on Queries
                        .SingleOrDefaultAsync();
    }

    async Task<RoomDto?> IRoomsRepository.GetWithDetails(Guid roomId, CancellationToken cancellationToken)
    {
         return await _dbContext.Rooms
            .Where(r => r.Id == roomId)
            .Select(room => RoomDto.MapToDto(room))
            .SingleOrDefaultAsync(cancellationToken);
    }
    async Task<IEnumerable<Room>?> IRoomsRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Rooms
                              .Include(r => r.Gym)
                              .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<Room>?> IRoomsRepository.ListByGymAsync(Guid gymId, CancellationToken cancellationToken)
    {
        return await _dbContext.Rooms
                               .Where(r => r.GymId == gymId)
                               .Include(r => r.Gym)
                               .ToListAsync(cancellationToken);
    }
    
    async Task<bool> IRoomsRepository.RoomHasOverlappingSession(Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
       
        var sessions = await _dbContext.Sessions
                    .Where(s =>
                        s.RoomId == roomId &&
                        start < s.EndDate &&
                        end > s.StartDate)
                    .ToListAsync();

        var exist = sessions.Any(s => s.IsActive());

        return exist;                                            
    }
}