using System.Text.Json.Serialization;

namespace ProjetoCep.Api.Models
{
    public class BrasilApiCepResponse
    {
        [JsonPropertyName("cep")]
        public string Cep { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string Uf { get; set; } = string.Empty; // Mapeado para UF

        [JsonPropertyName("city")]
        public string Localidade { get; set; } = string.Empty; // Mapeado para Localidade

        [JsonPropertyName("neighborhood")]
        public string Bairro { get; set; } = string.Empty;

        [JsonPropertyName("street")]
        public string Logradouro { get; set; } = string.Empty;

        [JsonPropertyName("service")]
        public string Service { get; set; } = string.Empty; // Ex: "correios"

        [JsonPropertyName("location")]
        public BrasilApiLocation Location { get; set; } = new BrasilApiLocation();
    }
}