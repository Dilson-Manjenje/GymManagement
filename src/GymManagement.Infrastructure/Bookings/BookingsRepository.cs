using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Bookings;

internal class BookingsRepository : IBookingsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public BookingsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    async Task IBookingsRepository.AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(booking);
        await Task.CompletedTask;
    }


    async Task IBookingsRepository.UpdateAsync(Booking booking, CancellationToken cancellationToken)
    {
        _dbContext.Bookings.Update(booking);
        await Task.CompletedTask;
    }

    async Task IBookingsRepository.UpdateRangeAsync(IEnumerable<Booking> bookings, CancellationToken cancellationToken)
    {
        _dbContext.Bookings.UpdateRange(bookings);
        await Task.CompletedTask;
    }
    
    async Task<Booking?> IBookingsRepository.GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
         var booking = await _dbContext.Bookings
                            .Include( x => x.Member )
                            .Include( x => x.Session)
                                .ThenInclude(x => x.Room)
                            .Include( x => x.Session)
                                .ThenInclude(s => s.Trainer)
                            .AsSingleQuery()
                            .SingleOrDefaultAsync(x => x.Id == Id, cancellationToken);
        return booking;
    }

    async Task<IEnumerable<Booking>?> IBookingsRepository.ListAsync(CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Bookings
                           .Include(x => x.Member)
                           .Include(x => x.Session)
                               .ThenInclude(x => x.Room)
                           .Include(x => x.Session)
                               .ThenInclude(s => s.Trainer)
                           .AsSingleQuery()
                           .ToListAsync(cancellationToken);
        return bookings;
    }

    async Task<IEnumerable<Booking>?> IBookingsRepository.ListByGymAsync(Guid gymId, CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Bookings
                           .Where(b => b.Session.Room.GymId == gymId)
                           .Include(x => x.Member)
                           .Include(x => x.Session)
                               .ThenInclude(x => x.Room)
                           .Include(x => x.Session)
                               .ThenInclude(s => s.Trainer)
                           .AsSingleQuery()
                           .ToListAsync(cancellationToken);
        return bookings;
    }
    
    async Task<Booking?> IBookingsRepository.GetByMemberAndSessionAsync(Guid memberId, Guid sessionId, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.Bookings
                           .SingleOrDefaultAsync(b => b.MemberId == memberId
                                                   && b.SessionId == sessionId
                                                   && b.Status == BookingStatus.Active,
                                                   cancellationToken);

        return booking;
    }

    async Task<IEnumerable<Booking>?> IBookingsRepository.ListByMemberAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Bookings
                           .Where(b => b.MemberId == memberId)
                           .Include(x => x.Member)
                           .Include(x => x.Session)
                               .ThenInclude(x => x.Room)
                           .Include(x => x.Session)
                               .ThenInclude(s => s.Trainer)
                           .AsSingleQuery()
                           .ToListAsync(cancellationToken);

        return bookings;
    }

    async Task<IEnumerable<Booking>?> IBookingsRepository.ListActiveBookingsBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.Bookings
                           .Where(b => b.SessionId == sessionId &&
                                  b.Status == BookingStatus.Active)
                           .Include(x => x.Member)
                           .Include(x => x.Session)
                               .ThenInclude(x => x.Room)
                           .Include(x => x.Session)
                               .ThenInclude(s => s.Trainer)
                           .AsSingleQuery()
                           .ToListAsync(cancellationToken);

        return bookings;
    }
        
}