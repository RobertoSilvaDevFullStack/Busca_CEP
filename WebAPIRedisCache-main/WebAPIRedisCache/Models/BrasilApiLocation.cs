using System.Text.Json.Serialization;

namespace ProjetoCep.Api.Models
{
    public class BrasilApiLocation
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("coordinates")]
        public BrasilApiCoordinates Coordinates { get; set; } = new BrasilApiCoordinates();
    }
}