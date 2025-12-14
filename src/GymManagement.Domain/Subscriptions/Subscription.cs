using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Members;
using GymManagement.Domain.Common;

namespace GymManagement.Domain.Subscriptions
{
    public class Subscription : Entity
    {
        public SubscriptionType SubscriptionType { get; private set; } = SubscriptionType.Basic;
        public Guid MemberId { get; private set; }
        public Member Member { get; set; } = null!;
        public List<SubscriptionRooms> SubscriptionRooms { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }        
        public bool IsActive => EndDate >= DateTime.Today;                 
        public decimal Price => SubscriptionType.Price;
        public int MaxDailySessions => SubscriptionType.MaxDailySessions;       
        public int MaxRoomsAllowed => SubscriptionType.MaxRooms;
        public int NumberOfRooms => SubscriptionRooms.Count();
        
        public Subscription(SubscriptionType subscriptionType,
                            Guid memberId,
                            Guid? id = null) : base(id ?? Guid.NewGuid())
        {
            MemberId = memberId;
            SubscriptionType = subscriptionType;
            StartDate = DateTime.Now;
            EndDate = StartDate.AddDays(SubscriptionType.DurationInDays);
        }
        
        private Subscription()
        {
            
        }

        public ErrorOr<Updated> UpdateSubscription(SubscriptionType subscriptionType)
        {
            SubscriptionType = subscriptionType;

            return Result.Updated;
        }

        public bool HasRoom(Guid roomId)
        {
            var exist = SubscriptionRooms.Any(sr => sr.RoomId == roomId);
            return exist;
        }
       
        public ErrorOr<Success> AddRoom(Guid roomId)
        {
            var exist = SubscriptionRooms.Any(sr => sr.RoomId == roomId);
            if (exist)
                return SubscriptionErrors.RoomAlreadyAssociated(roomId);

            if (SubscriptionRooms.Count >= SubscriptionType.MaxRooms)
                return SubscriptionErrors.HasMaxRoomsAllowed(); 

            SubscriptionRooms.Add(new SubscriptionRooms(subscriptionId: Id, roomId: roomId));

            return Result.Success;
        }

        public ErrorOr<Success> RemoveRoom(Guid roomId)
        {
            var subscriptionRoom = SubscriptionRooms.SingleOrDefault(sr => sr.RoomId == roomId);
            if (subscriptionRoom is null)
                return SubscriptionErrors.RoomNotInSubscription(roomId);

            // TODO: Remove room physically from Db, allow add another time
            // Add Event to remove from database
            SubscriptionRooms.Remove(subscriptionRoom);

            return Result.Success;
        }

    }
}