using ProjetoCep.Api.Exceptions;
using ProjetoCep.Api.Models;

namespace ProjetoCep.Api.Clients
{
    public class BrasilApiClient : IBrasilApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrasilApiClient> _logger;

        public BrasilApiClient(HttpClient httpClient, ILogger<BrasilApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BrasilApiCepResponse?> GetCepAsync(string cep)
        {
            _logger.LogInformation("Consultando BrasilAPI para o CEP: {Cep}", cep);
            try
            {
                var response = await _httpClient.GetAsync(cep);

                if (response.IsSuccessStatusCode)
                {
                    var brasilApiCep = await response.Content.ReadFromJsonAsync<BrasilApiCepResponse>();
                    _logger.LogInformation("CEP {Cep} encontrado na BrasilAPI.", cep);
                    return brasilApiCep;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("CEP {Cep} não encontrado na BrasilAPI (Status 404).", cep);
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro ao consultar BrasilAPI para o CEP {Cep}. Status: {StatusCode}, Resposta: {Content}",
                                     cep, response.StatusCode, errorContent);
                    throw new BrasilApiException($"Erro na BrasilAPI para o CEP {cep}. Status: {response.StatusCode}.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede/HTTP ao consultar BrasilAPI para o CEP {Cep}.", cep);
                throw new BrasilApiException("Falha na conexão com a BrasilAPI.", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex, "Erro de desserialização da resposta da BrasilAPI para o CEP {Cep}.", cep);
                throw new BrasilApiException("Resposta inválida da BrasilAPI.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar BrasilAPI para o CEP {Cep}.", cep);
                throw new BrasilApiException("Erro inesperado ao consultar BrasilAPI.", ex);
            }
        }
    }
}