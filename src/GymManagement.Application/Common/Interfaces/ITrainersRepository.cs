using GymManagement.Domain.Trainers;

namespace GymManagement.Application.Common.Interfaces;

public interface ITrainersRepository
{
    Task AddAsync(Trainer trainer, CancellationToken cancellationToken = default);
    Task<Trainer?> GetByIdAsync(Guid trainerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trainer>?> ListAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(Trainer trainer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Trainer trainer, CancellationToken cancellationToken = default);
}