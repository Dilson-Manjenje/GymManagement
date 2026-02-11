using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.SmartEnum;

namespace GymManagement.Domain.Sessions
{
    public class SessionStatus: SmartEnum<SessionStatus>
    {
        public static readonly SessionStatus Scheduled = new (nameof(Scheduled), 1);
        public static readonly SessionStatus InProgress = new (nameof(InProgress), 2);
        public static readonly SessionStatus Canceled = new (nameof(Canceled), 3);
        public static readonly SessionStatus Finalized = new(nameof(Finalized), 4);

        public SessionStatus(string name, int value) : base(name, value)
        {
        }

        public static readonly HashSet<SessionStatus> NonCancelableStatus = new()
        {
            InProgress,
            Finalized,
            Canceled
        };
        
        public static readonly HashSet<SessionStatus> ActiveStatus = new()
        {
            Scheduled,
            InProgress
        };
    }
}