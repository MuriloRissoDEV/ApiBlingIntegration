using ApiBling.Application.DTOs;
using ApiBling.Application.Interfaces;
using ApiBling.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApiBling.Application.Services
{
    public class BlingApiService : IBlingApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://www.bling.com.br/Api/v3";

        public BlingApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri("https://www.bling.com.br/Api/v3/");
        }

        public async Task<BlingTokenResponse> GerarTokenAsync(string code)
        {
            var clientId = _configuration["Bling:ClientId"];
            var clientSecret = _configuration["Bling:ClientSecret"];
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "authorization_code"),
                new("code", code)
            };
            request.Content = new FormUrlEncodedContent(collection);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlingTokenResponse>(json)!;
        }

        public async Task<BlingTokenResponse> AtualizarTokenAsync(string refreshToken)
        {
            var clientId = _configuration["Bling:ClientId"];
            var clientSecret = _configuration["Bling:ClientSecret"];
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken)
            };
            request.Content = new FormUrlEncodedContent(collection);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlingTokenResponse>(json)!;
        }

        public async Task<BlingProdutoResponse?> ObterProdutosAsync(string accessToken, int pagina, int limite = 100)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"produtos?pagina={pagina}&limite={limite}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Isso garante que o C# consiga ler o JSON mesmo se o Bling mudar alguma letra pra maiúscula/minúscula
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<BlingProdutoResponse>(json, options);
        }
        public async Task<BlingPedidoResponse?> ObterPedidosAsync(string accessToken, int pagina, int limite = 100)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"pedidos/vendas?pagina={pagina}&limite={limite}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BlingPedidoResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<BlingPedidoDetalheResponse?> ObterPedidoDetalheAsync(string accessToken, long idPedido)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"pedidos/vendas/{idPedido}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // ADICIONADO: Imprime o JSON bruto no terminal
            Console.WriteLine($"[Bling API] JSON bruto do pedido {idPedido}: {json}");

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return System.Text.Json.JsonSerializer.Deserialize<BlingPedidoDetalheResponse>(json, options);
        }



        public async Task<BlingNotaFiscalResponse?> ObterNotasFiscaisAsync(string accessToken, int pagina)
        {
            // Usando a rota nfce (Nota Fiscal de Consumidor). Se for NFe padrão, mude para "nfe"
            var request = new HttpRequestMessage(HttpMethod.Get, $"nfce?pagina={pagina}&limite=100");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return System.Text.Json.JsonSerializer.Deserialize<BlingNotaFiscalResponse>(json, options);
        }

        public async Task<BlingNotaFiscalDetalheResponse?> ObterNotaFiscalDetalheAsync(string accessToken, long idNota)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"nfce/{idNota}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return System.Text.Json.JsonSerializer.Deserialize<BlingNotaFiscalDetalheResponse>(json, options);
        }
        public async Task<BlingProdutoDetalheResponse?> ObterProdutoDetalheAsync(string accessToken, long idProduto)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"produtos/{idProduto}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return System.Text.Json.JsonSerializer.Deserialize<BlingProdutoDetalheResponse>(json, options);
        }

        public async Task<IEnumerable<Categoria>> ObterCategoriasAsync(string accessToken)
        {
            var todasCategorias = new List<Categoria>();
            int pagina = 1;
            int limite = 100;
            bool temMaisPaginas = true;

            while (temMaisPaginas)
            {
                // Adicionamos os parâmetros de paginação na URL
                var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/categorias/produtos?pagina={pagina}&limite={limite}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<BlingCategoriaResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (resultado?.Data != null && resultado.Data.Any())
                {
                    foreach (var itemDto in resultado.Data)
                    {
                        todasCategorias.Add(new Categoria
                        {
                            IdBling = itemDto.Id,
                            Nome = itemDto.Descricao ?? string.Empty,
                            IdCategoriaPai = itemDto.IdCategoriaPai,
                            DataAtualizacao = System.DateTime.UtcNow
                        });
                    }

                    // Se a API trouxe menos itens do que o limite (100), significa que chegamos na última página
                    if (resultado.Data.Count < limite)
                    {
                        temMaisPaginas = false;
                    }
                    else
                    {
                        pagina++; // Vai para a próxima página
                    }
                }
                else
                {
                    // Se a lista veio vazia, acabou
                    temMaisPaginas = false;
                }

                // Rate Limit: Aguarda 400ms antes de pedir a próxima página para não tomar erro 429 (Too Many Requests)
                if (temMaisPaginas)
                {
                    await Task.Delay(400);
                }
            }

            return todasCategorias;
        }

        public async Task<List<BlingNaturezaOperacaoData>> ObterNaturezasOperacoesAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            var resultado = new List<BlingNaturezaOperacaoData>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/naturezas-operacoes");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            int pagina = 1;
            int limite = 100; // Substitui o antigo PageSize
            bool hasMore = true;

            while (hasMore)
            {
                using var pageRequest = await CloneRequestAsync(request, pagina, limite);
                using var response = await _httpClient.SendAsync(pageRequest, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Falha ao obter naturezas de operações: {response.StatusCode}");
                }

                var blingResponse = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<BlingNaturezaOperacaoResponse>(response.Content, cancellationToken: cancellationToken);

                if (blingResponse?.Data == null || blingResponse.Data.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                resultado.AddRange(blingResponse.Data);
                hasMore = blingResponse.Data.Count >= limite;
                pagina++;

                // Rate limit para não estourar a API do Bling ao virar a página
                if (hasMore)
                {
                    await Task.Delay(400, cancellationToken);
                }
            }

            return resultado;
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original, int pagina, int limite)
        {
            var uri = new UriBuilder(original.RequestUri!.AbsoluteUri)
            {
                Query = $"limite={limite}&pagina={pagina}"
            };

            var clone = new HttpRequestMessage(original.Method, uri.Uri);
            foreach (var header in original.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            return await Task.FromResult(clone);
        }

        public async Task<BlingNotaFiscalGeralResponse> ObterNotasFiscaisGeraisPaginaAsync(string accessToken, int pagina, int limite, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[Bling API] Buscando NFe - Página {pagina}...");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/nfe?pagina={pagina}&limite={limite}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var blingResponse = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<BlingNotaFiscalGeralResponse>(response.Content, cancellationToken: cancellationToken);

            return blingResponse ?? new BlingNotaFiscalGeralResponse();
        }

        public async Task<BlingNotaFiscalGeralDetalheResponse?> ObterNotaFiscalGeralDetalheAsync(string accessToken, long idNota, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/nfe/{idNota}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) return null;

            return await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<BlingNotaFiscalGeralDetalheResponse>(response.Content, cancellationToken: cancellationToken);
        }

        public async Task<List<BlingSituacaoData>> ObterSituacoesPedidoVendaAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            // 1. PRIMEIRO: Descobrir qual é o ID do módulo de Pedidos de Venda
            var requestModulos = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/situacoes/modulos");
            requestModulos.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestModulos.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var responseModulos = await _httpClient.SendAsync(requestModulos, cancellationToken);
            if (!responseModulos.IsSuccessStatusCode) return new List<BlingSituacaoData>();

            var jsonModulos = await responseModulos.Content.ReadAsStringAsync(cancellationToken);

            // Imprime no terminal para você ver os módulos e seus IDs!
            Console.WriteLine("[Bling API] Lista de Módulos retornada:");
            Console.WriteLine(jsonModulos);

            using var doc = JsonDocument.Parse(jsonModulos);
            long idModuloVendas = 0;

            // Procura o ID do módulo "Pedidos de Venda" ou "Vendas" no JSON
            foreach (var modulo in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                // O Bling pode chamar de "nome" ou "descricao" dependendo da rota
                var nomeModulo = modulo.TryGetProperty("nome", out var propNome) ? propNome.GetString() :
                                 modulo.TryGetProperty("descricao", out var propDesc) ? propDesc.GetString() : "";

                if (nomeModulo != null && nomeModulo.Contains("Venda", StringComparison.OrdinalIgnoreCase))
                {
                    idModuloVendas = modulo.GetProperty("id").GetInt64();
                    Console.WriteLine($"[Bling API] Módulo encontrado! ID: {idModuloVendas} - Nome: {nomeModulo}");
                    break;
                }
            }

            if (idModuloVendas == 0)
            {
                Console.WriteLine("[Bling API] Não encontrei o módulo de Pedidos de Venda.");
                return new List<BlingSituacaoData>();
            }

            // 2. SEGUNDO: Agora sim, buscar as situações usando o ID correto
            var requestSituacoes = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/situacoes/modulos/{idModuloVendas}");
            requestSituacoes.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestSituacoes.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var responseSituacoes = await _httpClient.SendAsync(requestSituacoes, cancellationToken);
            if (!responseSituacoes.IsSuccessStatusCode) return new List<BlingSituacaoData>();

            var blingResponse = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<BlingSituacaoResponse>(responseSituacoes.Content, cancellationToken: cancellationToken);

            return blingResponse?.data ?? new List<BlingSituacaoData>();
        }

        public async Task<BlingNotaFiscalServicoResponse> ObterNotasFiscaisServicoPaginaAsync(string accessToken, int pagina, int limite, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[Bling API] Buscando NFSe - Página {pagina}...");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/nfse?pagina={pagina}&limite={limite}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) return new BlingNotaFiscalServicoResponse();

            var blingResponse = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<BlingNotaFiscalServicoResponse>(response.Content, cancellationToken: cancellationToken);

            return blingResponse ?? new BlingNotaFiscalServicoResponse();
        }

     

    }
}



