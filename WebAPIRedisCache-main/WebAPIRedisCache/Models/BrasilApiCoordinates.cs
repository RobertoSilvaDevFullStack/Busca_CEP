using System.Text.Json.Serialization;

namespace ProjetoCep.Api.Models
{
    public class BrasilApiCoordinates
    {
        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = string.Empty;
    }
}