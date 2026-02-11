using System.Text.Json.Serialization;

namespace GymManagement.Contracts.Sessions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SessionStatusType
{
    Scheduled = 1,
    InProgress = 2,
    Canceled = 3,
    Finalized = 4
}