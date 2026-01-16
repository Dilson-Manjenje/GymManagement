using GymManagement.Domain.Gyms;

namespace GymManagement.Application.Common.Interfaces;

public interface IGymsRepository
{
    Task AddAsync(Gym gym, CancellationToken cancellationToken = default);
    Task UpdateAsync(Gym gym, CancellationToken cancellationToken = default);
    Task RemoveAsync(Gym gym, CancellationToken cancellationToken = default);    
    Task<Gym?> GetByIdAsync(Guid gymId, CancellationToken cancellationToken = default);
    // TODO: Add GetWithDetails, that return Dto with Room, Trainers and Members
    Task<IEnumerable<Gym>?> ListAsync(CancellationToken cancellationToken = default);
    
}
