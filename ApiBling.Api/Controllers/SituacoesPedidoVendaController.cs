using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SituacoesPedidoVendaController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public SituacoesPedidoVendaController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar(CancellationToken cancellationToken)
        {
            var total = await _sincronizacaoService.SincronizarSituacoesPedidoVendaAsync(cancellationToken);
            return Ok(new { mensagem = "Sincronização de Situações concluída.", totalRegistros = total });
        }
    }
}