using Microsoft.EntityFrameworkCore;
using GymManagement.Domain.Subscriptions;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Members;
using GymManagement.Domain.Trainers;
using GymManagement.Domain.Common;

namespace GymManagement.Infrastructure.Common.Persistence;

public class GymManagementDbContext : DbContext, IUnitOfWork
{
    public DbSet<Gym> Gyms { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Trainer> Trainers { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionRooms> SubscriptionRooms { get; set; } = null!;
    public GymManagementDbContext(DbContextOptions options) : base(options)
    {

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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //TODO: Change to use SaveChangeInterceptors
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified )
            {
                var now = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreationDate(now);
                
                if (entry.State == EntityState.Modified)
                    entry.Entity.SetLastUpdate(now);
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}