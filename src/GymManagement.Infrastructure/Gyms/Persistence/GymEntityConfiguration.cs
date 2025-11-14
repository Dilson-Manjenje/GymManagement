using GymManagement.Domain.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Gyms.Persistence;

public class GymEntityConfiguration : IEntityTypeConfiguration<Gym>
{
    public void Configure(EntityTypeBuilder<Gym> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
               .ValueGeneratedNever(); 

        builder.Property(g => g.Name)
               .HasMaxLength(60)
               .IsRequired();

        builder.Property(g => g.Address)
               .HasMaxLength(100)
               .IsRequired();
               
    }
}