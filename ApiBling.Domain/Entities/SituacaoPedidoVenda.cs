using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities
{
    public class SituacaoPedidoVenda
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public long IdHerdado { get; set; }
        public string Cor { get; set; } = string.Empty;
    }
}
