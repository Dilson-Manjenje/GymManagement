using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;

namespace GymManagement.Application.Common.Interfaces;

public interface ISessionsRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);
    Task RemoveAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<SessionDto?> GetSessionDetails(Guid sessionId, CancellationToken cancellationToken = default);    
    Task<IEnumerable<Session>?> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>?> ListByGymAsync(Guid gymId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>?> ListByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>?> ListByTrainer(Guid trainerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>?> ListByMember(Guid trainerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>?> ListUpCommingSessions(Guid gymId, CancellationToken cancellationToken = default);
}
