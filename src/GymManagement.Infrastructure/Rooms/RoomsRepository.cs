using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.Common.Interfaces;
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

    async Task<Room?> IRoomsRepository.GetByIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        //return await _dbContext.Rooms.FindAsync(roomId, cancellationToken);
        return await _dbContext.Rooms
                        .Where(r => r.Id == roomId)
                        .Include(r => r.Gym)
                        .SingleOrDefaultAsync();
    }

    async Task<IEnumerable<Room>?> IRoomsRepository.GetRoomsByGymIdAsync(Guid gymId, CancellationToken cancellationToken)
    {
        return await _dbContext.Rooms
                               .Where(r => r.GymId == gymId)
                               .Include(r => r.Gym)
                               .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<Room>?> IRoomsRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Rooms
                              .Include(r => r.Gym)    
                              .ToListAsync(cancellationToken);
    }

    async Task IRoomsRepository.UpdateAsync(Room room, CancellationToken cancellationToken)
    {
        _dbContext.Rooms.Update(room);
        await Task.CompletedTask;                       
    }
}