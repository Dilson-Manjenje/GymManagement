using GymManagement.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Subscriptions.Persistence;

public class SubscriptionRoomsConfiguration : IEntityTypeConfiguration<SubscriptionRooms>
{
    public void Configure(EntityTypeBuilder<SubscriptionRooms> builder)
    {
        builder.HasKey(sr => sr.Id);
        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.HasIndex(sr => new { sr.SubscriptionId, sr.RoomId })
                .IsUnique();
                
        // -- Many-To-Many: Subscription -> SubscriptionRoom <- Room
        builder.HasOne(sr => sr.Subscription)
               .WithMany(s => s.SubscriptionRooms)
               .HasForeignKey(sr => sr.SubscriptionId)
               .OnDelete(DeleteBehavior.Cascade); // Delete related SubscriptionRoom when delete Subscription

        builder.HasOne(sr => sr.Room)
              .WithMany() // No Room navigation property
              .HasForeignKey(sr => sr.RoomId)
              .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Room with SubscriptionRoom               
   
    }
}
