using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Application.DTOs
{
    public class BlingProdutoResponse
    {
        public List<BlingProdutoData> data { get; set; } = new();
    }

    public class BlingProdutoData
    {
        public long id { get; set; }
        public string nome { get; set; } = string.Empty;
        public string codigo { get; set; } = string.Empty;
        public decimal preco { get; set; }
        public decimal precoCusto { get; set; }
        public string tipo { get; set; } = string.Empty;
        public string situacao { get; set; } = string.Empty;
        public string formato { get; set; } = string.Empty;
        public string descricaoCurta { get; set; } = string.Empty;
        public string imagemURL { get; set; } = string.Empty;
        public long? idProdutoPai { get; set; }
        public BlingEstoque? estoque { get; set; }
    }

    public class BlingEstoque
    {
        public decimal saldoVirtualTotal { get; set; }
    }
}
