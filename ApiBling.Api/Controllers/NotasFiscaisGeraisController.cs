using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisGeraisController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public NotasFiscaisGeraisController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar(CancellationToken cancellationToken)
        {
            var total = await _sincronizacaoService.SincronizarNotasFiscaisGeraisAsync(cancellationToken);
            return Ok(new { mensagem = "Sincronização de NFe concluída.", totalRegistros = total });
        }

        [HttpPost("sincronizar-detalhes")]
        public async Task<IActionResult> SincronizarDetalhes(CancellationToken cancellationToken)
        {
            var total = await _sincronizacaoService.SincronizarDetalhesNotasFiscaisGeraisAsync(cancellationToken);
            return Ok(new { mensagem = "Sincronização de detalhes concluída.", totalRegistros = total });
        }
    }
}