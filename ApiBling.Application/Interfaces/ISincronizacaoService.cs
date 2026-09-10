namespace ApiBling.Application.Interfaces
{
    public interface ISincronizacaoService
    {
        Task SincronizarProdutosAsync(string? code = null);
        Task SincronizarPedidosAsync(string? code = null);

        Task SincronizarDetalhesPedidosAsync();

        Task SincronizarNotasFiscaisAsync();
        Task SincronizarDetalhesNotasFiscaisAsync();
        Task SincronizarDetalhesProdutosAsync();
        Task SincronizarCategoriasAsync();
        Task<int> SincronizarNaturezasOperacoesAsync(CancellationToken cancellationToken = default);
        Task<int> SincronizarNotasFiscaisGeraisAsync(CancellationToken cancellationToken = default);
        Task<int> SincronizarDetalhesNotasFiscaisGeraisAsync(CancellationToken cancellationToken = default);
        Task<int> SincronizarSituacoesPedidoVendaAsync(CancellationToken cancellationToken = default);
        Task<int> SincronizarNotasFiscaisServicoAsync(CancellationToken cancellationToken = default);
        

    }
}