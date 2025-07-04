using ProjetoCep.Api.Models;

namespace ProjetoCep.Api.Services
{
    public interface ICepService
    {
        Task<CepResponseDto> GetCepInfoAsync(string cep);
    }
}