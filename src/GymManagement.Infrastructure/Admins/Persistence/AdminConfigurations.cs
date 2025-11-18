using GymManagement.Domain.Admins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Admins.Persistence;

public class AdminConfigurations : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
         builder.HasKey(a => a.Id);
 
        builder.Property(a => a.Id)
                .ValueGeneratedNever();

        builder.Property(a => a.UserId);
        builder.Property(a => a.UserName)
               .HasMaxLength(60)
               .IsRequired();
               
        builder.Property(a => a.SubscriptionId);

        builder.HasData(new Admin(
            userName: "admin",
            userId: Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"),
            subscriptionId: null,
            id: Guid.Parse("7d555faf-06b9-409f-a3ba-60d2a6bfc228")));
    }
}
