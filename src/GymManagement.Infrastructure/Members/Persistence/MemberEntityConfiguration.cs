using GymManagement.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Members.Persistence;

public class MemberEntityConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {

       builder.HasKey(x => x.Id);

              builder.Property(x => x.Id)
                      .ValueGeneratedNever();

              builder.Property(x => x.UserId);
              builder.Property(x => x.UserName)
                     .HasMaxLength(60)
                     .IsRequired();

              builder.Property(x => x.GymId)
                     .IsRequired(false);

              // --- One-To-Many: Gym -> Members      
              builder.HasOne(x => x.Gym)
                   .WithMany() //.WithMany(g => g.Members)                   
                   .HasForeignKey(x => x.GymId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting gym with member/users     

              // builder.Property(x => x.CreationDate)
              //        .IsRequired();

       }
}
