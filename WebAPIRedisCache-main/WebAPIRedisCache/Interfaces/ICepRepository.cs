using ProjetoCep.Api.Models;

namespace ProjetoCep.Api.Repositories
{
    public interface ICepRepository
    {
        Task<CepEntity?> GetByCepAsync(string cep);
        Task AddOrUpdateAsync(CepEntity cepEntity);
    }
}