using Microsoft.EntityFrameworkCore;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;
using GymManagement.Infrastructure.Common.Persistence;
using GymManagement.Domain.Sessions;

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
                    .Include(t => t.Gym)
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

    async Task<bool> ITrainersRepository.IsTrainerInGymAsync(Guid gymId, Guid memberId)
    {
        var exist = await _dbContext.Trainers.AnyAsync(t => t.GymId == gymId && t.MemberId == memberId);
        return exist;
    }

    async Task<bool> ITrainersRepository.HasSessionAsync(Guid trainerId)
    {
        var exist = await _dbContext.Sessions.AnyAsync(x => x.TrainerId == trainerId);
                                            // && (x.Status == SessionStatus.Scheduled || x.Status == SessionStatus.InProgress ));
        return exist;
    }
    
    async Task<bool> ITrainersRepository.ExistsWithEmailAsync(Guid gymId, string email, Guid? trainerId)
    {
        var emailValue = email?.ToLower(); 
        var exist = await _dbContext.Trainers
                .AnyAsync(t =>
                    t.GymId == gymId &&
                    (t.Email == emailValue) &&
                    (trainerId == null || t.Id != trainerId));
                    
        return exist;
    }

    async Task<bool> ITrainersRepository.ExistsWithPhoneAsync(Guid gymId, string phone, Guid? trainerId)
    {
        var exist = await _dbContext.Trainers
              .AnyAsync(t =>
                  t.GymId == gymId &&
                  t.Phone == phone &&
                  (trainerId == null || t.Id != trainerId));
        
        return exist;
    }
}