using Microsoft.AspNetCore.Mvc;
using ProjetoCep.Api.Exceptions;
using ProjetoCep.Api.Services;

namespace WebAPIRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CepController : ControllerBase
    {
        private readonly ICepService _cepService;
        private readonly ILogger<CepController> _logger;

        public CepController(ICepService cepService, ILogger<CepController> logger)
        {
            _cepService = cepService;
            _logger = logger;
        }

        [HttpGet("{cep}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<IActionResult> GetCep([FromRoute] string cep)
        {
            _logger.LogInformation("Requisição recebida para o CEP: {Cep}", cep);
            try
            {
                var cepInfo = await _cepService.GetCepInfoAsync(cep);
                return Ok(cepInfo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida para o CEP {Cep}.", cep);
                return BadRequest(new { message = ex.Message });
            }
            catch (CepNotFoundException ex)
            {
                _logger.LogWarning(ex, "CEP {Cep} não encontrado.", cep);
                return NotFound(new { message = ex.Message });
            }
            catch (BrasilApiException ex)
            {
                _logger.LogError(ex, "Erro na integração com a BrasilAPI para o CEP {Cep}.", cep);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao consultar a API externa de CEP. Tente novamente mais tarde." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado ao processar a requisição para o CEP {Cep}.", cep);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." + ex.Message);
            }
        }
    }
}