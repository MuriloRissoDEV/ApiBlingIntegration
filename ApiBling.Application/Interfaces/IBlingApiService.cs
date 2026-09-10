using ApiBling.Application.DTOs;
using ApiBling.Domain.Entities;

namespace ApiBling.Application.Interfaces
{
    public interface IBlingApiService
    {
        Task<BlingTokenResponse> GerarTokenAsync(string code);
        Task<BlingTokenResponse> AtualizarTokenAsync(string refreshToken);
        Task<BlingProdutoResponse?> ObterProdutosAsync(string accessToken, int pagina, int limite = 100);
        Task<BlingPedidoResponse?> ObterPedidosAsync(string accessToken, int pagina, int limite = 100);
        Task<BlingPedidoDetalheResponse?> ObterPedidoDetalheAsync(string accessToken, long idPedido);
        Task<BlingNotaFiscalResponse?> ObterNotasFiscaisAsync(string accessToken, int pagina);
        Task<BlingNotaFiscalDetalheResponse?> ObterNotaFiscalDetalheAsync(string accessToken, long idNota);
        Task<BlingProdutoDetalheResponse?> ObterProdutoDetalheAsync(string accessToken, long idProduto);
        Task<IEnumerable<Categoria>> ObterCategoriasAsync(string accessToken);
        Task<List<BlingNaturezaOperacaoData>> ObterNaturezasOperacoesAsync(string accessToken, CancellationToken cancellationToken = default);
        Task<BlingNotaFiscalGeralResponse> ObterNotasFiscaisGeraisPaginaAsync(string accessToken, int pagina, int limite, CancellationToken cancellationToken = default);
        Task<BlingNotaFiscalGeralDetalheResponse?> ObterNotaFiscalGeralDetalheAsync(string accessToken, long idNota, CancellationToken cancellationToken = default);
        Task<List<BlingSituacaoData>> ObterSituacoesPedidoVendaAsync(string accessToken, CancellationToken cancellationToken = default);
        Task<BlingNotaFiscalServicoResponse> ObterNotasFiscaisServicoPaginaAsync(string accessToken, int pagina, int limite, CancellationToken cancellationToken = default);
       
    }
}