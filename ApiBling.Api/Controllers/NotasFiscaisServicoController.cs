using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisServicoController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public NotasFiscaisServicoController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar(CancellationToken cancellationToken)
        {
            var total = await _sincronizacaoService.SincronizarNotasFiscaisServicoAsync(cancellationToken);
            return Ok(new { mensagem = "Sincronização de NFSe concluída.", totalRegistros = total });
        }
    }
}