using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Admins.Persistence;

internal class AdminsRepository : IAdminsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public AdminsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    async Task<Admin?> IAdminsRepository.GetByIdAsync(Guid adminId, CancellationToken cancellationToken)
    {
        return await _dbContext.Admins
                    .Include(a => a.Subscriptions)
                    .Include(a => a.Trainer)
                    .Include(a => a.Gym)
                    .FirstOrDefaultAsync(a => a.Id == adminId);
    }
    async Task<IEnumerable<Admin>?> IAdminsRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Admins
                    .Include(a => a.Subscriptions )
                    .Include(a => a.Trainer)
                    .Include(a => a.Gym)
                    .ToListAsync();
    }
    async Task IAdminsRepository.UpdateAsync(Admin admin, CancellationToken cancellationToken)
    {
        _dbContext.Admins.Update(admin);
        await Task.CompletedTask;
    }

    async Task IAdminsRepository.AddAsync(Admin admin, CancellationToken cancellationToken)
    {
        await _dbContext.Admins.AddAsync(admin);        
    }
}