using GymManagement.Domain.Members;

namespace GymManagement.Application.Common.Interfaces;

public interface IMembersRepository
{
    Task<Member?> GetByIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Member member, CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
    Task<IEnumerable<Member>?> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Member>?> ListByGymAsync(Guid gymId, CancellationToken cancellationToken = default);
}