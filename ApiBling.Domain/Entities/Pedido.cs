using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public long BlingId { get; set; }
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public int SituacaoId { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public DateTime DataUltimaSincronizacao { get; set; } = DateTime.UtcNow;

        public decimal TotalProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorDesconto { get; set; }
        public string? Observacoes { get; set; }
        public bool DetalhesSincronizados { get; set; } = false;
        public string? ServicoVolume { get; set; }
        public string? CodigoRastreamento { get; set; }

        public List<PedidoItem> Itens { get; set; } = new();
    }
}