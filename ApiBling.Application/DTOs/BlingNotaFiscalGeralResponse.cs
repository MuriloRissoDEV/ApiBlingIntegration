using System;
using System.Collections.Generic;

namespace ApiBling.Application.DTOs
{
    public class BlingNotaFiscalGeralResponse
    {
        public List<BlingNotaFiscalGeralData> data { get; set; } = new();
    }

    public class BlingNotaFiscalGeralData
    {
        public long id { get; set; }
        public int tipo { get; set; }
        public int situacao { get; set; }
        public string numero { get; set; } = string.Empty;
        public string? dataEmissao { get; set; }
        public string? dataOperacao { get; set; }
        public string chaveAcesso { get; set; } = string.Empty;

        public BlingNaturezaOperacaoRef? naturezaOperacao { get; set; }
        public BlingLojaRef? loja { get; set; }
    }

    public class BlingNaturezaOperacaoRef
    {
        public long id { get; set; }
    }

    public class BlingLojaRef
    {
        public long id { get; set; }
    }
}
