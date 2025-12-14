using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagement.Domain.Subscriptions;

public static class SubscriptionTypeConst
{
    public const int DurationInDays = 30;
    public static class Basic
    {
        public const decimal Price = 20000;
        public const int MaxRooms = 1;
        public const int MaxDailySessions = 1;

    }

    public static class Plus
    {
        public const decimal Price = 30000;
        public const int MaxRooms = 2;
        public const int MaxDailySessions = 3;

    }
    
    public static class Premium
    {
        public const decimal Price = 50000;
        public const int MaxRooms = 30;
        public const int MaxDailySessions = 10;
        
    }                
}

