using MongoDB.Driver;
using ProjetoCep.Api.Models;

namespace ProjetoCep.Api.Repositories
{
    public class CepRepository : ICepRepository
    {
        private readonly IMongoCollection<CepEntity> _cepsCollection;
        private readonly ILogger<CepRepository> _logger;

        public CepRepository(IMongoClient mongoClient, IConfiguration configuration, ILogger<CepRepository> logger)
        {
            _logger = logger;
            var databaseName = configuration["MongoDbConnection:DatabaseName"];
            var database = mongoClient.GetDatabase(databaseName);
            _cepsCollection = database.GetCollection<CepEntity>("BR");
        }

        public async Task<CepEntity?> GetByCepAsync(string cep)
        {
            _logger.LogInformation("Buscando CEP {Cep} no MongoDB.", cep);
            try
            {
                return await _cepsCollection.Find(c => c.Cep == cep).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CEP {Cep} no MongoDB.", cep);
                throw;
            }
        }

        public async Task AddOrUpdateAsync(CepEntity cepEntity)
        {
            _logger.LogInformation("Persistindo CEP {Cep} no MongoDB.", cepEntity.Cep);
            try
            {
                var filter = Builders<CepEntity>.Filter.Eq(c => c.Cep, cepEntity.Cep);

                var options = new ReplaceOptions { IsUpsert = true };

                await _cepsCollection.ReplaceOneAsync(filter, cepEntity, options);

                _logger.LogInformation("CEP {Cep} gravado no MongoDB.", cepEntity.Cep);
            }
            catch (MongoWriteException ex)
            {
                _logger.LogError(ex, "Erro de escrita no MongoDB para o CEP {Cep}.", cepEntity.Cep);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao persistir CEP {Cep} no MongoDB.", cepEntity.Cep);
                throw;
            }
        }
    }
}