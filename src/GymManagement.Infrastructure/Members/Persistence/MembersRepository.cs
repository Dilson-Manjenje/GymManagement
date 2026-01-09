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
                    .Include(m => m.Subscriptions)
                    .Include(m => m.Trainer)
                    .Include(m => m.Gym)
                    //.Include(m => m.Bookings)
                    .SingleOrDefaultAsync(a => a.Id == memberId);
    }
    async Task<IEnumerable<Member>?> IMembersRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Members
                    .Include(m => m.Subscriptions)
                    .Include(m => m.Trainer)
                    .Include(m => m.Gym)
                    //.Include(m => m.Bookings)
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

    async Task<IEnumerable<Member>?> IMembersRepository.ListByGymAsync(Guid gymId, CancellationToken cancellationToken)
    {
        return await _dbContext.Members
                        .Include(m => m.Subscriptions)
                        .Include(m => m.Trainer)
                        .Include(m => m.Gym)
                        //.Include(m => m.Bookings)
                        .Where(m => m.GymId == gymId).ToListAsync();
    }
}