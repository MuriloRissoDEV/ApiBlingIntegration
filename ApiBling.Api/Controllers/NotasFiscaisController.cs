using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiBling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public NotasFiscaisController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar()
        {
            try
            {
                await _sincronizacaoService.SincronizarNotasFiscaisAsync();
                return Ok(new { mensagem = "Notas Fiscais sincronizadas com sucesso!" });
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
                await _sincronizacaoService.SincronizarDetalhesNotasFiscaisAsync();
                return Ok(new { mensagem = "Detalhes das Notas Fiscais sincronizados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}