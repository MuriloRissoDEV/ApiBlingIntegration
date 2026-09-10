using ApiBling.Application.DTOs;
using ApiBling.Application.Interfaces;
using ApiBling.Domain.Entities;
using ApiBling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Net.Http.Headers;

namespace ApiBling.Application.Services
{
    public class SincronizacaoService : ISincronizacaoService
    {
        private readonly IBlingApiService _blingApiService;
        private readonly AppDbContext _dbContext;

        public SincronizacaoService(IBlingApiService blingApiService, AppDbContext dbContext)
        {
            _blingApiService = blingApiService;
            _dbContext = dbContext;
        }

        public async Task SincronizarProdutosAsync(string? code = null)
        {
            // 1. Gerencia o Token (Cria, Renova ou Reutiliza)
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            string accessToken = "";

            if (tokenAtual == null && string.IsNullOrEmpty(code))
                throw new Exception("Nenhum token encontrado. É necessário passar o 'code' na primeira requisição.");

            if (tokenAtual == null && !string.IsNullOrEmpty(code))
            {
                var novoToken = await _blingApiService.GerarTokenAsync(code);
                tokenAtual = new BlingToken
                {
                    AccessToken = novoToken.access_token,
                    RefreshToken = novoToken.refresh_token,
                    ExpiresIn = novoToken.expires_in,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.BlingTokens.Add(tokenAtual);
                await _dbContext.SaveChangesAsync();
            }
            else if (tokenAtual != null && tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            accessToken = tokenAtual!.AccessToken;

            // 2. Loop de Paginação Inteligente
            int pagina = 1;
            bool temMaisProdutos = true;

            while (temMaisProdutos)
            {
                var response = await _blingApiService.ObterProdutosAsync(accessToken, pagina);

                if (response?.data == null || !response.data.Any())
                {
                    temMaisProdutos = false; // Fim das páginas
                    break;
                }

                foreach (var item in response.data)
                {
                    var produtoExistente = await _dbContext.Produtos.FirstOrDefaultAsync(p => p.BlingId == item.id);

                    if (produtoExistente == null)
                    {
                        _dbContext.Produtos.Add(new Produto
                        {
                            BlingId = item.id,
                            Nome = item.nome,
                            Codigo = item.codigo,
                            Preco = item.preco,
                            PrecoCusto = item.precoCusto,
                            Tipo = item.tipo,
                            Situacao = item.situacao,
                            Formato = item.formato,
                            DescricaoCurta = item.descricaoCurta,
                            ImagemURL = item.imagemURL,
                            IdProdutoPai = item.idProdutoPai,
                            Estoque = item.estoque?.saldoVirtualTotal ?? 0,
                            DataUltimaSincronizacao = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        produtoExistente.Nome = item.nome;
                        produtoExistente.Codigo = item.codigo;
                        produtoExistente.Preco = item.preco;
                        produtoExistente.PrecoCusto = item.precoCusto;
                        produtoExistente.Estoque = item.estoque?.saldoVirtualTotal ?? 0;
                        produtoExistente.DataUltimaSincronizacao = DateTime.UtcNow;
                    }
                }

                await _dbContext.SaveChangesAsync();
                pagina++;
            }
        }

        public async Task SincronizarPedidosAsync(string? code = null)
        {
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");
            if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token; tokenAtual.RefreshToken = tokenRenovado.refresh_token; tokenAtual.ExpiresIn = tokenRenovado.expires_in; tokenAtual.CreatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
            int pagina = 1; bool temMaisPedidos = true;
            while (temMaisPedidos)
            {
                var response = await _blingApiService.ObterPedidosAsync(tokenAtual.AccessToken, pagina);
                if (response?.data == null || !response.data.Any()) { temMaisPedidos = false; break; }
                foreach (var item in response.data)
                {
                    var pedidoExistente = await _dbContext.Pedidos.FirstOrDefaultAsync(p => p.BlingId == item.id);
                    DateTime.TryParse(item.data, out DateTime dataPedido);
                    if (pedidoExistente == null)
                    {
                        _dbContext.Pedidos.Add(new Pedido { BlingId = item.id, Numero = item.numero, Data = dataPedido, Total = item.total, SituacaoId = item.situacao.id, NomeCliente = item.contato.nome, DataUltimaSincronizacao = DateTime.UtcNow });
                    }
                    else
                    {
                        pedidoExistente.Numero = item.numero; pedidoExistente.Data = dataPedido; pedidoExistente.Total = item.total; pedidoExistente.SituacaoId = item.situacao.id; pedidoExistente.NomeCliente = item.contato.nome; pedidoExistente.DataUltimaSincronizacao = DateTime.UtcNow;
                    }
                }
                await _dbContext.SaveChangesAsync(); pagina++;
            }
        }

        public async Task SincronizarDetalhesPedidosAsync()
        {
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

            if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var pedidosSemDetalhe = await _dbContext.Pedidos
                .Include(p => p.Itens)
                .Where(p => !p.DetalhesSincronizados)
                .ToListAsync();

            foreach (var pedido in pedidosSemDetalhe)
            {
                var response = await _blingApiService.ObterPedidoDetalheAsync(tokenAtual.AccessToken, pedido.BlingId);

                if (response?.data != null)
                {
                    var detalhe = response.data;

                    pedido.TotalProdutos = detalhe.totalProdutos;
                    pedido.ValorFrete = detalhe.transporte?.frete ?? 0;
                    pedido.ValorDesconto = detalhe.desconto?.valor ?? 0;
                    pedido.Observacoes = detalhe.observacoes;
                    pedido.DetalhesSincronizados = true;

                    // LOOP DOS ITENS (não mexer)
                    foreach (var itemBling in detalhe.itens)
                    {
                        var itemExistente = pedido.Itens.FirstOrDefault(i => i.BlingId == itemBling.id);
                        if (itemExistente == null)
                        {
                            pedido.Itens.Add(new PedidoItem
                            {
                                BlingId = itemBling.id,
                                Codigo = itemBling.codigo,
                                Descricao = itemBling.descricao,
                                Quantidade = itemBling.quantidade,
                                Valor = itemBling.valor
                            });
                        }
                        else
                        {
                            itemExistente.Quantidade = itemBling.quantidade;
                            itemExistente.Valor = itemBling.valor;
                            itemExistente.Descricao = itemBling.descricao;
                        }
                    }

                    // LÓGICA DOS VOLUMES (agora acessando através de transporte)
                    if (detalhe.transporte?.volumes != null && detalhe.transporte.volumes.Count > 0)
                    {
                        pedido.ServicoVolume = detalhe.transporte.volumes[0].servico;

                        if (detalhe.transporte.volumes.Count == 1)
                        {
                            pedido.CodigoRastreamento = detalhe.transporte.volumes[0].codigoRastreamento;
                        }
                        else
                        {
                            pedido.CodigoRastreamento = string.Join("; ",
                                detalhe.transporte.volumes
                                    .Where(v => !string.IsNullOrEmpty(v.codigoRastreamento))
                                    .Select(v => v.codigoRastreamento));
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        public async Task SincronizarNotasFiscaisAsync()
        {
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

            if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            int pagina = 1;
            bool temMaisNotas = true;

            while (temMaisNotas)
            {
                var response = await _blingApiService.ObterNotasFiscaisAsync(tokenAtual.AccessToken, pagina);

                if (response?.data == null || !response.data.Any())
                {
                    temMaisNotas = false;
                    break;
                }

                foreach (var item in response.data)
                {
                    var notaExistente = await _dbContext.NotasFiscais.FirstOrDefaultAsync(n => n.BlingId == item.id);

                    DateTime.TryParse(item.dataEmissao, out var dataEmissao);
                    DateTime? dataOperacao = DateTime.TryParse(item.dataOperacao, out var dOp) ? dOp : null;

                    if (notaExistente == null)
                    {
                        _dbContext.NotasFiscais.Add(new NotaFiscal
                        {
                            BlingId = item.id,
                            Numero = item.numero,
                            DataEmissao = dataEmissao,
                            DataOperacao = dataOperacao,
                            LojaId = item.loja?.id ?? 0,
                            ClienteNome = item.contato?.nome ?? "",
                            NaturezaOperacaoId = item.naturezaOperacao?.id ?? 0
                        });
                    }
                    else
                    {
                        notaExistente.ClienteNome = item.contato?.nome ?? "";
                        notaExistente.NaturezaOperacaoId = item.naturezaOperacao?.id ?? 0;
                    }

                    // Salva dentro do loop para evitar erro de chave duplicada
                    await _dbContext.SaveChangesAsync();
                }

                pagina++;
                await Task.Delay(400); // Delay de segurança na listagem
            }
        }

        public async Task SincronizarDetalhesNotasFiscaisAsync()
        {
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

            var notasSemDetalhe = await _dbContext.NotasFiscais
                .Include(n => n.Itens)
                .Where(n => !n.DetalhesSincronizados)
                .ToListAsync();

            foreach (var nota in notasSemDetalhe)
            {
                // 1. Renovação do token DENTRO do loop para evitar erro 401 em bases grandes
                if (tokenAtual.IsExpired)
                {
                    var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                    tokenAtual.AccessToken = tokenRenovado.access_token;
                    tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                    tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                    tokenAtual.CreatedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }

                try
                {
                    var response = await _blingApiService.ObterNotaFiscalDetalheAsync(tokenAtual.AccessToken, nota.BlingId);

                    if (response?.data != null)
                    {
                        var detalhe = response.data;


                        nota.ValorNota = detalhe.valorNota;
                        nota.ValorFrete = detalhe.valorFrete;
                        nota.OptanteSimplesNacional = detalhe.tributacao?.optanteSimplesNacional ?? false;

                        nota.DetalhesSincronizados = true;

                        foreach (var itemBling in detalhe.itens)
                        {
                            var itemExistente = nota.Itens.FirstOrDefault(i => i.CodigoProduto == itemBling.codigo);
                            if (itemExistente == null)
                            {
                                nota.Itens.Add(new NotaFiscalItem
                                {
                                    CodigoProduto = itemBling.codigo,
                                    Descricao = itemBling.descricao,
                                    Quantidade = itemBling.Quantidade,
                                    Valor = itemBling.Valor,
                                    ValorTotal = itemBling.ValorTotal > 0 ? itemBling.ValorTotal : (itemBling.Quantidade * itemBling.Valor)
                                });
                            }
                            else
                            {
                                // Se o item já existir, atualiza os valores
                                itemExistente.Quantidade = itemBling.Quantidade;
                                itemExistente.Valor = itemBling.Valor;
                                itemExistente.ValorTotal = itemBling.ValorTotal > 0 ? itemBling.ValorTotal : (itemBling.Quantidade * itemBling.Valor);
                            }
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Se der erro 403, marca como sincronizado para pular e não travar a fila
                    nota.DetalhesSincronizados = true;
                    nota.ClienteNome = "ERRO 403 - " + nota.ClienteNome;
                }
                catch (Exception)
                {
                    // Se der qualquer outro erro, pula também
                    nota.DetalhesSincronizados = true;
                }

                await _dbContext.SaveChangesAsync();

                // Pausa obrigatória para evitar erro 429
                await Task.Delay(400);
            }
        }
        public async Task SincronizarDetalhesProdutosAsync()
        {
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

            var produtosSemDetalhe = await _dbContext.Produtos
                .Where(p => !p.DetalhesSincronizados)
                .ToListAsync();

            foreach (var produto in produtosSemDetalhe)
            {
                if (tokenAtual.IsExpired)
                {
                    var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                    tokenAtual.AccessToken = tokenRenovado.access_token;
                    tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                    tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                    tokenAtual.CreatedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }

                try
                {
                    var response = await _blingApiService.ObterProdutoDetalheAsync(tokenAtual.AccessToken, produto.BlingId);

                    if (response?.data != null)
                    {
                        var detalhe = response.data;

                        produto.Ncm = detalhe.tributacao?.ncm ?? "";
                        produto.Cest = detalhe.tributacao?.cest ?? "";
                        produto.IdCategoriaBling = detalhe.categoria?.id;

                        produto.DetalhesSincronizados = true;
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    produto.DetalhesSincronizados = true;
                    produto.Ncm = "ERRO 403";
                }
                catch (Exception)
                {
                    produto.DetalhesSincronizados = true;
                    produto.Ncm = "ERRO";
                }

                await _dbContext.SaveChangesAsync();

                await Task.Delay(400);
            }
        }

        public async Task SincronizarCategoriasAsync()
        {
            // 1. LÓGICA DE TOKEN
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            string accessToken = "";

            if (tokenAtual == null)
            {
                throw new Exception("Nenhum token encontrado.");
            }
            else if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;

                _dbContext.BlingTokens.Update(tokenAtual);
                await _dbContext.SaveChangesAsync();

                accessToken = tokenAtual.AccessToken;
            }
            else
            {
                accessToken = tokenAtual.AccessToken;
            }

            // 2. CHAMA A API DO BLING COM O TOKEN GARANTIDO
            var categoriasBling = await _blingApiService.ObterCategoriasAsync(accessToken);

            if (categoriasBling == null)
            {
                return;
            }

            // 3. SALVA NO BANCO DE DADOS
            foreach (var categoria in categoriasBling)
            {
                var existente = await _dbContext.Categorias.FirstOrDefaultAsync(c => c.IdBling == categoria.IdBling);

                if (existente == null)
                {
                    _dbContext.Categorias.Add(categoria);
                }
                else
                {
                    existente.Nome = categoria.Nome;
                    existente.Tipo = categoria.Tipo;
                    existente.IdCategoriaPai = categoria.IdCategoriaPai;
                    existente.DataAtualizacao = DateTime.UtcNow;

                    _dbContext.Categorias.Update(existente);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> SincronizarNaturezasOperacoesAsync(CancellationToken cancellationToken = default)
        {
            // 1. LÓGICA DE TOKEN (Igual a que você já usa)
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);
            string accessToken = "";

            if (tokenAtual == null)
            {
                throw new Exception("Nenhum token encontrado.");
            }
            else if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;

                _dbContext.BlingTokens.Update(tokenAtual);
                await _dbContext.SaveChangesAsync(cancellationToken);

                accessToken = tokenAtual.AccessToken;
            }
            else
            {
                accessToken = tokenAtual.AccessToken;
            }

            // 2. BUSCA AS NATUREZAS DE OPERAÇÃO NA API
            var naturezasBling = await _blingApiService.ObterNaturezasOperacoesAsync(accessToken, cancellationToken);

            if (naturezasBling == null || naturezasBling.Count == 0)
            {
                return 0;
            }

            // 3. SALVA NO BANCO DE DADOS (UPSERT)
            var idsBling = naturezasBling.Select(n => n.Id).ToList();
            var existentes = await _dbContext.NaturezasOperacoes
                .Where(n => idsBling.Contains(n.Id))
                .ToDictionaryAsync(n => n.Id, cancellationToken);

            var agora = DateTime.UtcNow;
            int inseridos = 0;
            int atualizados = 0;

            foreach (var blingNatureza in naturezasBling)
            {
                if (existentes.TryGetValue(blingNatureza.Id, out var existente))
                {
                    existente.Descricao = blingNatureza.Descricao;
                    existente.Tipo = blingNatureza.Tipo;
                    existente.DataAlteracao = agora;

                    _dbContext.NaturezasOperacoes.Update(existente);
                    atualizados++;
                }
                else
                {
                    _dbContext.NaturezasOperacoes.Add(new NaturezaOperacao
                    {
                        Id = blingNatureza.Id,
                        Descricao = blingNatureza.Descricao,
                        Tipo = blingNatureza.Tipo,
                        DataCriacao = agora,
                        DataAlteracao = agora
                    });
                    inseridos++;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return inseridos + atualizados;
        }

        public async Task<int> SincronizarNotasFiscaisGeraisAsync(CancellationToken cancellationToken = default)
        {
            int pagina = 1;
            int limite = 100;
            bool hasMore = true;
            int totalInseridos = 0;
            int totalAtualizados = 0;

            while (hasMore)
            {
                // 1. LÓGICA DE TOKEN
                var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);
                string accessToken = "";

                if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

                if (tokenAtual.IsExpired)
                {
                    var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                    tokenAtual.AccessToken = tokenRenovado.access_token;
                    tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                    tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                    tokenAtual.CreatedAt = DateTime.UtcNow;

                    _dbContext.BlingTokens.Update(tokenAtual);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    // Limpa a memória após atualizar o token
                    _dbContext.ChangeTracker.Clear();

                    accessToken = tokenAtual.AccessToken;
                }
                else
                {
                    accessToken = tokenAtual.AccessToken;
                }

                // 2. BUSCA APENAS UMA PÁGINA NA API
                var responseBling = await _blingApiService.ObterNotasFiscaisGeraisPaginaAsync(accessToken, pagina, limite, cancellationToken);
                var notasBling = responseBling?.data;

                if (notasBling == null || notasBling.Count == 0)
                {
                    Console.WriteLine($"[Bling API] Fim das notas fiscais alcançado na página {pagina}.");
                    break;
                }

                // CORREÇÃO 1: Remove duplicatas que o Bling possa ter mandado na mesma página
                var notasUnicas = notasBling.GroupBy(n => n.id).Select(g => g.First()).ToList();

                // 3. PREPARA PARA SALVAR NO BANCO DE DADOS
                var idsBling = notasUnicas.Select(n => n.id).ToList();
                var existentes = await _dbContext.NotasFiscaisGerais
                    .Where(n => idsBling.Contains(n.Id))
                    .ToDictionaryAsync(n => n.Id, cancellationToken);

                foreach (var blingNota in notasUnicas)
                {
                    if (existentes.TryGetValue(blingNota.id, out var existente))
                    {
                        existente.Tipo = blingNota.tipo;
                        existente.Situacao = blingNota.situacao;
                        existente.Numero = blingNota.numero;
                        existente.DataEmissao = ConverterDataBling(blingNota.dataEmissao);
                        existente.DataOperacao = ConverterDataBling(blingNota.dataOperacao);
                        existente.ChaveAcesso = blingNota.chaveAcesso;
                        existente.IdNaturezaOperacao = blingNota.naturezaOperacao?.id;
                        existente.IdLoja = blingNota.loja?.id;

                        _dbContext.NotasFiscaisGerais.Update(existente);
                        totalAtualizados++;
                    }
                    else
                    {
                        _dbContext.NotasFiscaisGerais.Add(new NotaFiscalGeral
                        {
                            Id = blingNota.id,
                            Tipo = blingNota.tipo,
                            Situacao = blingNota.situacao,
                            Numero = blingNota.numero,
                            DataEmissao = ConverterDataBling(blingNota.dataEmissao),
                            DataOperacao = ConverterDataBling(blingNota.dataOperacao),
                            ChaveAcesso = blingNota.chaveAcesso,
                            IdNaturezaOperacao = blingNota.naturezaOperacao?.id,
                            IdLoja = blingNota.loja?.id
                        });
                        totalInseridos++;
                    }
                }

                // 4. SALVA NO BANCO A CADA PÁGINA
                await _dbContext.SaveChangesAsync(cancellationToken);

                // CORREÇÃO 2: Limpa a memória do Entity Framework para a próxima página!
                _dbContext.ChangeTracker.Clear();

                Console.WriteLine($"[Banco de Dados] Página {pagina} salva com sucesso! ({notasUnicas.Count} registros)");

                // 5. VERIFICA SE TEM MAIS PÁGINAS
                if (notasBling.Count < limite)
                {
                    hasMore = false;
                }
                else
                {
                    pagina++;
                }

                if (pagina > 1000) break; // Trava de segurança

                if (hasMore)
                {
                    await Task.Delay(400, cancellationToken);
                }
            }

            return totalInseridos + totalAtualizados;
        }
        private DateTime? ConverterDataBling(string? dataString)
        {
            if (string.IsNullOrWhiteSpace(dataString)) return null;
            if (DateTime.TryParse(dataString, out var data)) return data;
            return null;
        }

   


        public async Task<int> SincronizarDetalhesNotasFiscaisGeraisAsync(CancellationToken cancellationToken = default)
        {
            // Pega as notas que ainda não têm detalhes
            var notasSemDetalhe = await _dbContext.NotasFiscaisGerais
                .Where(n => !n.DetalhesSincronizados)
                .ToListAsync(cancellationToken);

            if (notasSemDetalhe.Count == 0) return 0;

            int atualizados = 0;

            foreach (var nota in notasSemDetalhe)
            {
                // 1. LÓGICA DE TOKEN (Renova a cada volta se precisar, pois o processo é longo)
                var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);
                string accessToken = "";

                if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

                if (tokenAtual.IsExpired)
                {
                    var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                    tokenAtual.AccessToken = tokenRenovado.access_token;
                    tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                    tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                    tokenAtual.CreatedAt = DateTime.UtcNow;

                    _dbContext.BlingTokens.Update(tokenAtual);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear(); // Limpa a memória
                    accessToken = tokenAtual.AccessToken;

                    // Recarrega a nota atual para a memória do EF após o Clear
                    _dbContext.NotasFiscaisGerais.Attach(nota);
                }
                else
                {
                    accessToken = tokenAtual.AccessToken;
                }

                // 2. BUSCA O DETALHE DA NOTA
                var detalheResponse = await _blingApiService.ObterNotaFiscalGeralDetalheAsync(accessToken, nota.Id, cancellationToken);
                var detalhe = detalheResponse?.data;

                if (detalhe != null)
                {
                    // Atualiza os campos do cabeçalho
                    nota.ValorFrete = detalhe.valorFrete;
                    nota.ValorTotal = detalhe.valorNota;
                    nota.Finalidade = ConverterInteiroBling(detalhe.finalidade);
                    nota.TipoNota = ConverterInteiroBling(detalhe.tipoNota);

                    // Remove itens antigos (se houver)
                    var itensExistentes = await _dbContext.NotasFiscaisGeraisItens.Where(i => i.IdNotaFiscalGeral == nota.Id).ToListAsync(cancellationToken);
                    if (itensExistentes.Any()) _dbContext.NotasFiscaisGeraisItens.RemoveRange(itensExistentes);

                    // Adiciona os novos itens
                    if (detalhe.itens != null)
                    {
                        foreach (var itemDto in detalhe.itens)
                        {
                            _dbContext.NotasFiscaisGeraisItens.Add(new NotaFiscalGeralItem
                            {
                                IdNotaFiscalGeral = nota.Id,
                                Codigo = itemDto.codigo,
                                Descricao = itemDto.descricao,
                                Unidade = itemDto.unidade,
                                Quantidade = itemDto.quantidade,
                                Valor = itemDto.valor,
                                ValorTotal = itemDto.quantidade * itemDto.valor, // Calculado
                                Tipo = itemDto.tipo
                            });
                        }
                    }

                    nota.DetalhesSincronizados = true;
                    _dbContext.NotasFiscaisGerais.Update(nota);

                    // Salva no banco A CADA NOTA para não perder progresso
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    atualizados++;

                    Console.WriteLine($"[Banco de Dados] Detalhes da NFe {nota.Numero} salvos com sucesso!");
                }

                // Rate limit do Bling (400ms)
                await Task.Delay(400, cancellationToken);
            }

            return atualizados;
        }

        private int? ConverterInteiroBling(string? valorString)
        {
            if (string.IsNullOrWhiteSpace(valorString)) return null;
            if (int.TryParse(valorString, out var valor)) return valor;
            return null;
        }

        public async Task<int> SincronizarSituacoesPedidoVendaAsync(CancellationToken cancellationToken = default)
        {
            // 1. LÓGICA DE TOKEN
            var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);
            string accessToken = "";

            if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

            if (tokenAtual.IsExpired)
            {
                var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                tokenAtual.AccessToken = tokenRenovado.access_token;
                tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                tokenAtual.CreatedAt = DateTime.UtcNow;

                _dbContext.BlingTokens.Update(tokenAtual);
                await _dbContext.SaveChangesAsync(cancellationToken);

                accessToken = tokenAtual.AccessToken;
            }
            else
            {
                accessToken = tokenAtual.AccessToken;
            }

            // 2. BUSCA AS SITUAÇÕES NA API
            var situacoesBling = await _blingApiService.ObterSituacoesPedidoVendaAsync(accessToken, cancellationToken);

            if (situacoesBling == null || situacoesBling.Count == 0) return 0;

            // 3. SALVA NO BANCO DE DADOS (UPSERT)
            var idsBling = situacoesBling.Select(s => s.id).ToList();
            var existentes = await _dbContext.SituacoesPedidosVenda
                .Where(s => idsBling.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, cancellationToken);

            int inseridos = 0;
            int atualizados = 0;

            foreach (var dto in situacoesBling)
            {
                if (existentes.TryGetValue(dto.id, out var existente))
                {
                    existente.Nome = dto.nome;
                    existente.IdHerdado = dto.idHerdado;
                    existente.Cor = dto.cor ?? string.Empty;

                    _dbContext.SituacoesPedidosVenda.Update(existente);
                    atualizados++;
                }
                else
                {
                    _dbContext.SituacoesPedidosVenda.Add(new SituacaoPedidoVenda
                    {
                        Id = dto.id,
                        Nome = dto.nome,
                        IdHerdado = dto.idHerdado,
                        Cor = dto.cor ?? string.Empty
                    });
                    inseridos++;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return inseridos + atualizados;
        }

        public async Task<int> SincronizarNotasFiscaisServicoAsync(CancellationToken cancellationToken = default)
        {
            int pagina = 1;
            int limite = 100;
            bool hasMore = true;
            int totalInseridos = 0;
            int totalAtualizados = 0;

            while (hasMore)
            {
                // 1. LÓGICA DE TOKEN
                var tokenAtual = await _dbContext.BlingTokens.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);
                string accessToken = "";

                if (tokenAtual == null) throw new Exception("Nenhum token encontrado.");

                if (tokenAtual.IsExpired)
                {
                    var tokenRenovado = await _blingApiService.AtualizarTokenAsync(tokenAtual.RefreshToken);
                    tokenAtual.AccessToken = tokenRenovado.access_token;
                    tokenAtual.RefreshToken = tokenRenovado.refresh_token;
                    tokenAtual.ExpiresIn = tokenRenovado.expires_in;
                    tokenAtual.CreatedAt = DateTime.UtcNow;

                    _dbContext.BlingTokens.Update(tokenAtual);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();

                    accessToken = tokenAtual.AccessToken;
                }
                else
                {
                    accessToken = tokenAtual.AccessToken;
                }

                // 2. BUSCA UMA PÁGINA NA API
                var responseBling = await _blingApiService.ObterNotasFiscaisServicoPaginaAsync(accessToken, pagina, limite, cancellationToken);
                var notasBling = responseBling?.data;

                if (notasBling == null || notasBling.Count == 0)
                {
                    Console.WriteLine($"[Bling API] Fim das NFSe alcançado na página {pagina}.");
                    break;
                }

                // Remove duplicatas que o Bling possa mandar
                var notasUnicas = notasBling.GroupBy(n => n.id).Select(g => g.First()).ToList();

                // 3. SALVA NO BANCO DE DADOS (UPSERT)
                var idsBling = notasUnicas.Select(n => n.id).ToList();
                var existentes = await _dbContext.NotasFiscaisServico
                    .Where(n => idsBling.Contains(n.Id))
                    .ToDictionaryAsync(n => n.Id, cancellationToken);

                foreach (var blingNota in notasUnicas)
                {
                    if (existentes.TryGetValue(blingNota.id, out var existente))
                    {
                        existente.Numero = blingNota.numero;
                        existente.NumeroRPS = blingNota.numeroRPS;
                        existente.Serie = blingNota.serie;
                        existente.Situacao = blingNota.situacao;
                        existente.DataEmissao = ConverterDataBling(blingNota.dataEmissao); // Usando o conversor!
                        existente.Valor = blingNota.valor;

                        existente.IdContato = blingNota.contato?.id;
                        existente.NomeContato = blingNota.contato?.nome ?? string.Empty;
                        existente.DocumentoContato = blingNota.contato?.numeroDocumento ?? string.Empty;
                        existente.EmailContato = blingNota.contato?.email ?? string.Empty;

                        _dbContext.NotasFiscaisServico.Update(existente);
                        totalAtualizados++;
                    }
                    else
                    {
                        _dbContext.NotasFiscaisServico.Add(new NotaFiscalServico
                        {
                            Id = blingNota.id,
                            Numero = blingNota.numero,
                            NumeroRPS = blingNota.numeroRPS,
                            Serie = blingNota.serie,
                            Situacao = blingNota.situacao,
                            DataEmissao = ConverterDataBling(blingNota.dataEmissao), // Usando o conversor!
                            Valor = blingNota.valor,

                            IdContato = blingNota.contato?.id,
                            NomeContato = blingNota.contato?.nome ?? string.Empty,
                            DocumentoContato = blingNota.contato?.numeroDocumento ?? string.Empty,
                            EmailContato = blingNota.contato?.email ?? string.Empty
                        });
                        totalInseridos++;
                    }
                }

                // 4. SALVA NO BANCO A CADA PÁGINA
                await _dbContext.SaveChangesAsync(cancellationToken);
                _dbContext.ChangeTracker.Clear(); // Limpa a memória do EF

                Console.WriteLine($"[Banco de Dados] Página {pagina} de NFSe salva com sucesso! ({notasUnicas.Count} registros)");

                // 5. VERIFICA SE TEM MAIS PÁGINAS
                if (notasBling.Count < limite)
                {
                    hasMore = false;
                }
                else
                {
                    pagina++;
                }

                if (pagina > 1000) break; // Trava de segurança

                if (hasMore)
                {
                    await Task.Delay(400, cancellationToken);
                }
            }

            return totalInseridos + totalAtualizados;
        }

    } 
}





