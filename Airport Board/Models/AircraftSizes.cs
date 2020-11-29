using System.Text.Json.Serialization;

namespace Airport_Board.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum AircraftSizes
    {        
        Small,
        Middle,
        Big
    }
}
