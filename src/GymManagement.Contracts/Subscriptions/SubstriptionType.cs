using System.Text.Json.Serialization;

namespace GymManagement.Contracts.Subscriptions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubstriptionType
{
    Basic = 1,
    Plus = 2,
    Premium = 3
}