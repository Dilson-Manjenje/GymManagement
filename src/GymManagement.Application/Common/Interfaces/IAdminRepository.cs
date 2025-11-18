using GymManagement.Domain.Admins;

namespace GymManagement.Application.Common.Interfaces;

public interface IAdminsRepository
{
    Task<Admin?> GetByIdAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Admin admin, CancellationToken cancellationToken = default);
    Task AddAsync(Admin admin, CancellationToken cancellationToken = default);
    Task<IEnumerable<Admin>?> ListAsync(CancellationToken cancellationToken = default);
    //Task<IEnumerable<Admin>?> ListByGym(Guid GymId, CancellationToken cancellationToken = default);
}