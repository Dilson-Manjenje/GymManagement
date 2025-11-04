using GymManagement.Domain.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Rooms.Persistence;

public class RoomRepositoryEntityConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedNever(); 

        builder.Property(r => r.Name)
               .HasMaxLength(60)
               .IsRequired();

       builder.Property(r => r.Capacity)
              .IsRequired();

       builder.Property(r => r.IsAvailable)
              .IsRequired();

       builder.Property( r => r.GymId)
                     .IsRequired();

       // builder.OwnsOne( r => r.Gym, gym =>
       // {
       //      gym.Property( gym  => gym.Id)
       //         .HasColumnName("GymId")
       //         .IsRequired();
       //  });              
               
    }
}