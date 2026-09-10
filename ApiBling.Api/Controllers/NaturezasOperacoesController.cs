using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiBling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NaturezasOperacoesController : ControllerBase
{
    private readonly ISincronizacaoService _sincronizacaoService;

    public NaturezasOperacoesController(ISincronizacaoService sincronizacaoService)
    {
        _sincronizacaoService = sincronizacaoService;
    }

    [HttpPost("sincronizar")]
    public async Task<IActionResult> Sincronizar(CancellationToken cancellationToken)
    {
        var total = await _sincronizacaoService.SincronizarNaturezasOperacoesAsync(cancellationToken);
        return Ok(new { mensagem = "Sincronização concluída.", totalRegistros = total });
    }
}