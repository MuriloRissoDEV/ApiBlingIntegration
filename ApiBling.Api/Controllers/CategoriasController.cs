using ApiBling.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SeuProjeto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ISincronizacaoService _sincronizacaoService;

        public CategoriasController(ISincronizacaoService sincronizacaoService)
        {
            _sincronizacaoService = sincronizacaoService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar()
        {
            await _sincronizacaoService.SincronizarCategoriasAsync();
            return Ok(new { mensagem = "Sincronização de categorias concluída com sucesso." });
        }
    }
}