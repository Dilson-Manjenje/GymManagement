using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Sessions;

internal class SessionsRepository : ISessionsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public SessionsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task ISessionsRepository.AddAsync(Session session, CancellationToken cancellationToken)
    {
        await _dbContext.Sessions.AddAsync(session, cancellationToken);
    }
        
    async Task  ISessionsRepository.UpdateAsync(Session session, CancellationToken cancellationToken)
    {
        _dbContext.Sessions.Update(session);
        await Task.CompletedTask; 
    }

    async Task ISessionsRepository.RemoveAsync(Session session, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.Sessions.Remove(session));        
    }

    async Task<Session?> ISessionsRepository.GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions
                            .Include(s => s.Room)
                                .ThenInclude(r => r.Gym)
                            .Include(s => s.Trainer)
                            .AsSingleQuery()
                            .SingleOrDefaultAsync(x => x.Id == Id, cancellationToken);
        return session;
    }

    async Task<SessionDto?> ISessionsRepository.GetSessionDetails(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions
                            .AsNoTracking()
                            .Where(s => s.Id == sessionId)
                            .Select(s => new SessionDto(s.Id,
                                                               s.Title,
                                                               s.Room.Gym.Name,
                                                               s.Room.GymId,
                                                               s.RoomId,
                                                               s.Room.Name,
                                                               s.TrainerId,
                                                               s.Trainer.Name,
                                                               s.Capacity,
                                                               s.Vacancy,
                                                               s.StartDate,
                                                               s.EndDate,
                                                               s.Status))
                            .SingleOrDefaultAsync();

        return session;
    
    }
    async Task<IEnumerable<Session>?> ISessionsRepository.ListUpCommingSessions(Guid gymId, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.Sessions
                                .Where(s => s.Room.GymId == gymId &&
                                        s.EndDate >= DateTime.Now &&
                                        (s.Status == SessionStatus.Scheduled ||
                                         s.Status == SessionStatus.InProgress) )
                                .OrderByDescending(s => s.StartDate)
                                .Include(s => s.Room)
                                    .ThenInclude(r => r.Gym)
                                .Include(s => s.Trainer)
                                .AsSingleQuery()
                                .ToListAsync(cancellationToken);

        return sessions;

    }
    
    async Task<IEnumerable<Session>?> ISessionsRepository.ListByGymAsync(Guid gymId, CancellationToken cancellationToken)
    {
         var sessions = await _dbContext.Sessions
                                .Where(s => s.Room.GymId == gymId )
                                .OrderByDescending(s => s.StartDate)
                                .Include(s => s.Room)
                                    .ThenInclude(r => r.Gym)
                                .Include(s => s.Trainer)
                                .AsSingleQuery()
                                .ToListAsync(cancellationToken);

        return sessions;
    }
    
    async Task<IEnumerable<Session>?> ISessionsRepository.ListAsync(CancellationToken cancellationToken)
    {
         return await _dbContext.Sessions
                                .OrderByDescending(s => s.StartDate)
                                .Include(s => s.Room)
                                    .ThenInclude(r => r.Gym)
                                .Include(s => s.Trainer)
                                .AsSingleQuery()
                                // .Select(s => new SessionDetailsDto(s.Id,
                                //                                s.Title,
                                //                                s.Room.Gym.Name,
                                //                                s.Room.GymId,
                                //                                s.RoomId,
                                //                                s.Room.Name,
                                //                                s.TrainerId,
                                //                                s.Trainer.Name,
                                //                                s.Capacity,
                                //                                s.Vacancy,
                                //                                s.StartDate,
                                //                                s.EndDate,
                                //                                s.Status))
                                .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<Session>?> ISessionsRepository.ListByRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.Sessions
                               .Where(s => s.RoomId == roomId)                    
                               .OrderByDescending(s => s.StartDate)
                               .Include(s => s.Room)         
                                    .ThenInclude(r => r.Gym)          
                               .Include(s => s.Trainer)                         
                               .AsSingleQuery()
                               .ToListAsync(cancellationToken);
        return sessions;                                
    }

    async Task<IEnumerable<Session>?> ISessionsRepository.ListByTrainer(Guid trainerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Sessions
                               .Where(s => s.TrainerId == trainerId)
                               .OrderByDescending(s => s.StartDate)
                               .Include(s => s.Room)
                                    .ThenInclude(r => r.Gym)
                               .Include(s => s.Trainer)
                               .AsSingleQuery()
                               .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<Session>?> ISessionsRepository.ListByMember(Guid memberId, CancellationToken cancellationToken)
    {
        return await _dbContext.Bookings
                               .Where(b => b.MemberId == memberId)
                               .OrderByDescending(b => b.Session.CreationDate)
                               .Include(b => b.Session)
                                    .ThenInclude(s => s.Room)
                                    .ThenInclude(r => r.Gym)
                               .Include(b => b.Session.Trainer)
                               .Select(b => b.Session)
                               .AsSingleQuery()
                               .ToListAsync(cancellationToken);

    }
}