using GymManagement.Domain.Trainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Trainers.Persistence;

public class TrainerEntityConfiguration : IEntityTypeConfiguration<Trainer>
{
       public void Configure(EntityTypeBuilder<Trainer> builder)
       {
              builder.HasKey(t => t.Id);

              builder.Property(t => t.Id)
                     .ValueGeneratedNever();

              builder.Property(t => t.Name)
                     .HasMaxLength(60)
                     .IsRequired();

              builder.Property(t => t.Email)
                     .HasMaxLength(100);

              builder.HasIndex(t => t.Email)
                     .IsUnique();

              builder.Property(t => t.Phone)
                     .HasMaxLength(15)
                     .IsRequired();

              builder.HasIndex(t => t.Phone)
                     .IsUnique();

              builder.Property(t => t.Specialization)
                   .HasMaxLength(100)
                   .IsRequired();

              builder.Property(t => t.GymId).IsRequired();

              // One-to-Many: Gym <-> Trainer               
              builder.HasOne(t => t.Gym)
                   .WithMany(g => g.Trainers)
                   .HasForeignKey(t => t.GymId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting gym with trainers

              // One-to-One: Trainer <-> Admin
              builder.HasOne(t => t.Admin)
               .WithOne(ad => ad.Trainer)
               .HasForeignKey<Trainer>(t => t.AdminId)
               .OnDelete(DeleteBehavior.Restrict); // Prevents deleting admin with trainer


       }
}