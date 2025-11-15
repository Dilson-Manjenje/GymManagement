using GymManagement.Domain.Admins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Admins.Persistence;

public class AdminConfigurations : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasData(new Admin(
            userId: Guid.NewGuid(),
            id: Guid.Parse("7D555FAF-06B9-409F-A3BA-60D2A6BFC228")));
    }
}
