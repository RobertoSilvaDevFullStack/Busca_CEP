using Microsoft.Extensions.Caching.Distributed;
using ProjetoCep.Api.Clients;
using ProjetoCep.Api.Exceptions;
using ProjetoCep.Api.Extensions;
using ProjetoCep.Api.Models;
using ProjetoCep.Api.Repositories;
using System.Text.RegularExpressions;

namespace ProjetoCep.Api.Services
{
    public class CepService : ICepService
    {
        private readonly ICepRepository _cepRepository;
        private readonly IBrasilApiClient _brasilApiClient;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CepService> _logger;

        public CepService(
            ICepRepository cepRepository,
            IBrasilApiClient brasilApiClient,
            IDistributedCache cache,
            ILogger<CepService> logger)
        {
            _cepRepository = cepRepository;
            _brasilApiClient = brasilApiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CepResponseDto> GetCepInfoAsync(string cep)
        {
            if (!IsValidCepFormat(cep))
            {
                _logger.LogWarning("Tentativa de busca com CEP inválido: {Cep}", cep);
                throw new ArgumentException("Formato de CEP inválido. Use 8 dígitos numéricos.");
            }

            CepEntity? cepEntity = null;

            // Passo 1: Tenta buscar no cache Redis
            _logger.LogInformation("Tentando buscar CEP {Cep} no Redis Cache.", cep);
            cepEntity = await _cache.GetAsync<CepEntity>(cep);

            if (cepEntity != null)
            {
                _logger.LogInformation("CEP {Cep} encontrado no Redis Cache.", cep);
                return CepResponseDto.FromEntity(cepEntity);
            }

            // Passo 2: Se não está no cache, tenta buscar no MongoDB
            _logger.LogInformation("CEP {Cep} não encontrado no cache. Tentando buscar no banco de dados.", cep);
            cepEntity = await _cepRepository.GetByCepAsync(cep);

            if (cepEntity != null)
            {
                _logger.LogInformation("CEP {Cep} encontrado no banco de dados. Armazenando no Redis Cache.", cep);
                // Salva no cache para futuras buscas serem mais rápidas
                await _cache.SetAsync(cep, cepEntity);
                return CepResponseDto.FromEntity(cepEntity);
            }

            // Passo 3: Se não está em nenhum banco, consulta a API externa
            _logger.LogInformation("CEP {Cep} não encontrado no banco de dados. Consultando BrasilAPI.", cep);
            BrasilApiCepResponse? brasilApiCep = null;
            try
            {
                brasilApiCep = await _brasilApiClient.GetCepAsync(cep);
            }
            catch (BrasilApiException ex)
            {
                _logger.LogError(ex, "Falha na comunicação com a BrasilAPI para o CEP {Cep}.", cep);
                throw;
            }

            if (brasilApiCep == null)
            {
                _logger.LogWarning("CEP {Cep} não encontrado na BrasilAPI.", cep);
                throw new CepNotFoundException($"CEP {cep} não encontrado.");
            }

            // Constrói a entidade para salvar nos bancos de dados
            cepEntity = new CepEntity
            {
                Cep = brasilApiCep.Cep.Replace("-", ""),
                Logradouro = brasilApiCep.Logradouro,
                Bairro = brasilApiCep.Bairro,
                Localidade = brasilApiCep.Localidade,
                Uf = brasilApiCep.Uf,
                Complemento = null,
                Ibge = null,
                Gia = null,
                Ddd = null,
                Siafi = null
            };

            // Salva o novo CEP no MongoDB e no Cache para futuras requisições
            _logger.LogInformation("Persistindo e armazenando no cache CEP {Cep} obtido da BrasilAPI.", cep);
            await _cepRepository.AddOrUpdateAsync(cepEntity);
            await _cache.SetAsync(cep, cepEntity);

            return CepResponseDto.FromEntity(cepEntity);
        }

        private bool IsValidCepFormat(string cep)
        {
            return Regex.IsMatch(cep, @"^\d{8}$");
        }
    }
}