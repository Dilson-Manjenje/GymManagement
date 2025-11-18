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
        await _dbContext.Trainers.AddAsync(trainer);
    }


    async Task<IEnumerable<Trainer>?> ITrainersRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Trainers.ToListAsync();
    }

    async Task ITrainersRepository.RemoveAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        _dbContext.Trainers.Remove(trainer);
        await Task.CompletedTask;            
    }

    async Task<Trainer?> ITrainersRepository.GetByIdAsync(Guid trainerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Trainers.FindAsync(trainerId, cancellationToken);
    }

    async Task ITrainersRepository.UpdateAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        _dbContext.Trainers.Update(trainer);
        await Task.CompletedTask;                       
    }
}