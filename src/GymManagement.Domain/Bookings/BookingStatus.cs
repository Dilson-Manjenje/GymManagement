using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.SmartEnum;

namespace GymManagement.Domain.Bookings
{
    public class BookingStatus: SmartEnum<BookingStatus>
    {
        public static readonly BookingStatus Active = new (nameof(Active), 1);
        public static readonly BookingStatus Canceled = new (nameof(Canceled), 2);
        public static readonly BookingStatus Finalized = new(nameof(Finalized), 3);

        public BookingStatus(string name, int value) : base(name, value)
        {
        }

        public static readonly HashSet<BookingStatus> NonCancelableStatus = new()
        {
            Finalized,
            Canceled
        };      

        public static readonly HashSet<BookingStatus> ActiveStatus = new()
        {
            Active            
        };  
    }
}