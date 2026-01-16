using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Trainers.Persistence;

internal class TraneirsRepository : ITrainersRepository
{
    private readonly GymManagementDbContext _dbContext;

    public TraneirsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task ITrainersRepository.AddAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        await _dbContext.Trainers.AddAsync(trainer, cancellationToken);
    }

        async Task ITrainersRepository.UpdateAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        _dbContext.Trainers.Update(trainer);
        await Task.CompletedTask;
    }
    
    async Task ITrainersRepository.RemoveAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        _dbContext.Trainers.Remove(trainer);
        await Task.CompletedTask;            
    }

    async Task<Trainer?> ITrainersRepository.GetByIdAsync(Guid trainerId, CancellationToken cancellationToken)
    {
        //return await _dbContext.Trainers.FindAsync(trainerId, cancellationToken);
        return await _dbContext.Trainers
                    .Where(t => t.Id == trainerId)
                    .Include(t => t.Gym)
                    .SingleOrDefaultAsync(cancellationToken);
    }

    async Task<Trainer?> ITrainersRepository.GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        return await _dbContext.Trainers
                    .Where(t => t.MemberId == memberId)
                    .SingleOrDefaultAsync(cancellationToken);
    }
    
    async Task<IEnumerable<Trainer>?> ITrainersRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Trainers
                                .Include(t => t.Gym)
                                .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<Trainer>?> ITrainersRepository.ListByGymIdAsync(Guid gymId, CancellationToken cancellationToken)
    {
        return await _dbContext.Trainers
                               .Where(t => t.GymId == gymId)
                               .Include(t => t.Gym)
                               .ToListAsync(cancellationToken);
    }

    bool ITrainersRepository.IsTrainerInGymAsync(Guid gymId, Guid memberId)
    {
        var exist =_dbContext.Trainers.Any(t => t.GymId == gymId && t.MemberId == memberId);
        return exist;
    }    
}