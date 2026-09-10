using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;

        public long BlingId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
