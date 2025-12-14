using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;


namespace GymManagement.Infrastructure.Members.Persistence;

internal class MembersRepository : IMembersRepository
{
    private readonly GymManagementDbContext _dbContext;

    public MembersRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    async Task<Member?> IMembersRepository.GetByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        return await _dbContext.Members
                    .Include(a => a.Subscriptions)
                    .Include(a => a.Trainer)
                    .Include(a => a.Gym)
                    .FirstOrDefaultAsync(a => a.Id == memberId);
    }
    async Task<IEnumerable<Member>?> IMembersRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Members
                    .Include(a => a.Subscriptions)
                    .Include(a => a.Trainer)
                    .Include(a => a.Gym)
                    .ToListAsync();
    }
    async Task IMembersRepository.UpdateAsync(Member member, CancellationToken cancellationToken)
    {
        _dbContext.Members.Update(member);
        await Task.CompletedTask;
    }

    async Task IMembersRepository.AddAsync(Member member, CancellationToken cancellationToken)
    {
        await _dbContext.Members.AddAsync(member);
    }

    async Task<IEnumerable<Member>?> IMembersRepository.ListByGym(Guid gymId, CancellationToken cancellationToken)
    {
        return await _dbContext.Members
                        .Include(m => m.Gym)
                        .Include(m => m.Trainer)
                        .Where(m => m.GymId == gymId).ToListAsync();
    }
}