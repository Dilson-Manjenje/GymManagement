using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Sessions;

public class SessionEntityConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id)
             .ValueGeneratedNever();

      builder.Property(x => x.Title)
              .HasMaxLength(60)
              .IsRequired();

      builder.Property(x => x.TrainerId)
             .IsRequired();

      // --- One-To-Many: Trainer -> Session
      builder.HasOne(x => x.Trainer)
           //.WithMany(x => x.Sessions)
           .WithMany()
              .HasForeignKey(x => x.TrainerId)
           .IsRequired(true) // Required
           .OnDelete(DeleteBehavior.Restrict); // Prevents deleting trainer with sessions

      // --- One-To-Many: Room -> Sessions
      builder.HasOne(x => x.Room)
           //.WithMany(x => x.Sessions)
           .WithMany()
              .HasForeignKey(x => x.RoomId)
           .IsRequired(true)
           .OnDelete(DeleteBehavior.Restrict); // Prevents deleting room with sessions

      builder.Property(x => x.Status)
              .HasConversion(
                   sessionStatus => sessionStatus.Value, 
                   value => SessionStatus.FromValue(value)) 
              .IsRequired();

   }
}