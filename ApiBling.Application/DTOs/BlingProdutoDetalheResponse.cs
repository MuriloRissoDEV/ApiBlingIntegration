using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Application.DTOs
{
    public class BlingProdutoDetalheResponse
    {
        public BlingProdutoDetalheData data { get; set; } = new();
    }

    public class BlingProdutoDetalheData
    {
        public long id { get; set; }
        public BlingProdutoTributacao? tributacao { get; set; }

        // NOVO CAMPO: Objeto categoria retornado no detalhe do produto
        public BlingCategoriaRef? categoria { get; set; }
    }

    public class BlingProdutoTributacao
    {
        public string ncm { get; set; } = string.Empty;
        public string cest { get; set; } = string.Empty;
    }

    // NOVA CLASSE: Para mapear o id da categoria que vem no JSON
    public class BlingCategoriaRef
    {
        public int id { get; set; }
    }
}
