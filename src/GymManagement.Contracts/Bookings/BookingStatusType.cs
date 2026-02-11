using System.Text.Json.Serialization;

namespace GymManagement.Contracts.Bookings;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatusType
{
    Active = 1,
    Canceled = 2,
    Finalized = 3
}