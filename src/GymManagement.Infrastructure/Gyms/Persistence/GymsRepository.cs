using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Gyms.Persistence;

internal class GymsRepository : IGymsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public GymsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task IGymsRepository.RemoveAsync(Gym gym, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.Gyms.Remove(gym));    
        
        // TODO: Disable instead of delete gym
        // await _dbContext.Gyms
        //     .Where(g => g.Id == gym.Id)
        //     .ExecuteDeleteAsync(cancellationToken); // Delete imediattly no wait for SaveChanges 

    }

    async Task IGymsRepository.AddAsync(Gym gym, CancellationToken cancellationToken)
    {
        await _dbContext.Gyms.AddAsync(gym);
    }

    async Task<Gym?> IGymsRepository.GetByIdAsync(Guid gymId, CancellationToken cancellationToken)
    {
       return await _dbContext.Gyms
                        .Where(g => g.Id == gymId)
                        .Include(g => g.Rooms)
                        //TODO: Include Trainer
                        .FirstOrDefaultAsync();
    }

    async Task<Gym?> IGymsRepository.GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Gyms
            .AsNoTracking()
            .FirstOrDefaultAsync(gym => gym.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    async Task<IEnumerable<Gym>?> IGymsRepository.ListAsync(CancellationToken cancellationToken)
    {
         return await _dbContext.Gyms
                        .AsNoTracking()
                        //.Include(g => g.Rooms) //TODO: Include Rooms and Trainer
                        .ToListAsync(cancellationToken);
    }

    async Task IGymsRepository.UpdateAsync(Gym gym, CancellationToken cancellationToken)
    {
        _dbContext.Gyms.Update(gym);
        await Task.CompletedTask;
    }
}