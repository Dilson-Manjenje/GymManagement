using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Infrastructure.Bookings;

public class BookingEntityConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);        
        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // --- One-To-Many: Member -> Booking
        builder.HasOne(x => x.Member)
             .WithMany()
             .HasForeignKey(x => x.MemberId)
             .IsRequired(true) 
             .OnDelete(DeleteBehavior.Restrict); // Prevents deleting member with booking

        // --- One-To-Many: Session -> Booking
        builder.HasOne(x => x.Session)
             .WithMany()
             .HasForeignKey(x => x.SessionId)
             .IsRequired(true)
             .OnDelete(DeleteBehavior.Restrict); // Prevents session room with booking 
             
         builder.Property(x => x.Status)
                       .HasConversion(
                            bookingStatus => bookingStatus.Value, // convertTo (writting to DB)
                            value => BookingStatus.FromValue(value)) // convertFrom (reading)
                       .IsRequired();                   
    }
}