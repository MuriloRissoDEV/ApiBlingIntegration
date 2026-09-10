using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public ProdutosController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar([FromQuery] string? code)
        {
            try
            {
                await _sincronizacaoService.SincronizarProdutosAsync(code);
                return Ok(new { mensagem = "Sincronização concluída com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }


        }

        [HttpPost("sincronizar-detalhes")]
        public async Task<IActionResult> SincronizarDetalhes()
        {
            try
            {
                await _sincronizacaoService.SincronizarDetalhesProdutosAsync();
                return Ok(new { mensagem = "Detalhes dos produtos (NCM e CEST) sincronizados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}