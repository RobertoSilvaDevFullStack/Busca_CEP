using ProjetoCep.Api.Models;

namespace ProjetoCep.Api.Clients
{
    public interface IBrasilApiClient
    {
        Task<BrasilApiCepResponse?> GetCepAsync(string cep);
    }
}