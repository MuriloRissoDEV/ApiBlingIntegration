using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public PedidosController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar()
        {
            try
            {
                await _sincronizacaoService.SincronizarPedidosAsync();
                return Ok(new { mensagem = "Sincronização de pedidos concluída com sucesso!" });
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
                await _sincronizacaoService.SincronizarDetalhesPedidosAsync();
                return Ok(new { mensagem = "Detalhes dos pedidos sincronizados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }

        }
    }
}

