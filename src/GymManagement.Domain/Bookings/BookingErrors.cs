using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;

namespace GymManagement.Domain.Bookings;

public static class BookingErrors
{
  public static Error BookingNotFound(Guid id) => Error.NotFound
    (code: "Booking.BookingNotFound",
      description: $"Booking with ID {id} not found.");
  public static Error CantChangeBooking(Guid id, string statusName) => Error.Validation
  (code: "Booking.CantChangeBooking",
      description: $"Cant change booking '{id}' with status {statusName}.");

  // public static Error CantFinalizeBookingWithActiveSession(Guid id) => Error.Validation
  // (code: "Booking.CantFinalizeBookingWithActiveSession",
  //    description: $"Cant finalize Booking {id} with Active Session. Finalize or Cancel the Session before.");
  
  public static Error InvalidBookingDetails() => Error.Validation
  (code: "Booking.InvalidBookingDetails",
     description: $"Invalid Booking Details. Need Session and Member information.");
  public static Error InvalidSessionsStatus(Guid id, string statusName) => Error.Validation
  (code: "Booking.InvalidSessionsStatus",
     description: $"Cant create or change Booking with Session '{id}' on status '{statusName}'.");

  public static Error MemberNotInTheSameGym(Guid memberId) => Error.NotFound
 (code: "Booking.MemberNotInTheSameGym",
   description: $"The Member with ID {memberId} is not in same Gym of the Session.");

  public static Error DuplicateBooking(Guid memberId, Guid sessionId) => Error.Conflict
 (code: "Booking.DuplicateBooking",
   description: $"The Member with ID {memberId} already booked for this Session {sessionId}.");

  public static Error MemberDontHaveActiveSubscription(Guid memberId) => Error.Validation
(code: "Booking.MemberDontHaveActiveSubscription",
 description: $"The Member with ID {memberId} dont have an active subscription.");
   
  public static Error SubscriptionDontHaveAccess(Guid subscriptionId, Guid roomId) => Error.Validation
 (code: "Booking.SubscriptionDontHaveAccess",
   description: $"Subscription {subscriptionId} dont have access to the room {roomId}.");
    

}