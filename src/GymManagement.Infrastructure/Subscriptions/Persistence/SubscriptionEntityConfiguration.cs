using GymManagement.Domain.Members;
using GymManagement.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Subscriptions.Persistence;

public class SubscriptionEntityConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)                
               .ValueGeneratedNever(); // Wont generated value when entity is saved
                                       // but can have temporary value before save the entity.

        builder.Property(s => s.MemberId)
               .IsRequired();

        // --- One-To-Many: Member -> Subscription
        builder.HasOne(s => s.Member)
             .WithMany(a => a.Subscriptions)
             .HasForeignKey(s => s.MemberId) 
             .IsRequired(true) // Required
             .OnDelete(DeleteBehavior.Restrict); // Prevents deleting member with subscription

          builder.Property(s => s.SubscriptionType)
                       .HasConversion(
                            subscriptiontype => subscriptiontype.Value, // convertTo (writting to DB)
                            value => SubscriptionType.FromValue(value)) // convertFrom (reading)
                                                                        //.HasDefaultValue(1)  // sets DB default to 1 (Basic)
                                                                        //.HasColumnType("INTEGER") // enforce column type
                       .IsRequired();    

        //   builder.Property(s => s.CreationDate)
        //       .HasDefaultValueSql("CURRENT_TIMESTAMP");
              
        //   builder.Property(s => s.LastUpdateDate)
        //       .HasDefaultValueSql("CURRENT_TIMESTAMP");     
    }
}