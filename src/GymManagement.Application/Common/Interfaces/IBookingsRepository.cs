using GymManagement.Domain.Bookings;

namespace GymManagement.Application.Common.Interfaces;

public interface IBookingsRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Booking> bookings, CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);    
    Task<Booking?> GetByMemberAndSessionAsync(Guid memberId, Guid sessionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>?> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>?> ListByGymAsync(Guid gymId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>?> ListByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);        
    Task<IEnumerable<Booking>?> ListActiveBookingsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);        
}