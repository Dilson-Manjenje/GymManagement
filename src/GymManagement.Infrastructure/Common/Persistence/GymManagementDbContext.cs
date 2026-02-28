using Microsoft.EntityFrameworkCore;
using GymManagement.Domain.Subscriptions;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Members;
using GymManagement.Domain.Trainers;
using GymManagement.Domain.Common;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Bookings;
using MediatR;

namespace GymManagement.Infrastructure.Common.Persistence;

public class GymManagementDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public DbSet<Gym> Gyms { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Trainer> Trainers { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionRooms> SubscriptionRooms { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;

    public GymManagementDbContext(DbContextOptions options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //TODO: Change to use SaveChangeInterceptors
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var now = DateTime.Now;
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreationDate(now);

                if (entry.State == EntityState.Modified)
                    entry.Entity.SetLastUpdate(now);
            }
        }
        
        // Publish domain events: 
        // - BEFORE SaveChanges if domain events are part of the same transaction (Immediate Consistency)
        // - AFTER SaveChanges if domain events are a separate transaction (Eventual Consistency)

        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entities = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.HasDomainEvents())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities.SelectMany(e => e.PopAndClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}