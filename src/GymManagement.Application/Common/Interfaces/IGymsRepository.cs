using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Gyms;

namespace GymManagement.Application.Common.Interfaces;

public interface IGymsRepository
{
    Task AddAsync(Gym gym, CancellationToken cancellationToken = default);
    Task<Gym?> GetByIdAsync(Guid gymId, CancellationToken cancellationToken = default);
    Task<Gym?> GetByName(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Gym>?> ListAsync(CancellationToken cancellationToken = default);
    Task RemoveGym(Gym gym, CancellationToken cancellationToken = default);
    Task UpdateAsync(Gym gym, CancellationToken cancellationToken = default);
}
