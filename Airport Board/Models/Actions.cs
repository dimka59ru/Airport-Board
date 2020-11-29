using System.Text.Json.Serialization;

namespace Airport_Board.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum Actions
    {
        Departure,
        Arrival
    }
}
