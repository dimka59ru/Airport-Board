using System.Text.Json.Serialization;

namespace Airport_Board.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum FlightActions
    {
        Departure,
        Arrival
    }
}
