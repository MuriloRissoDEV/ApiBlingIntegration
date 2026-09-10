using System.Collections.Generic;

namespace ApiBling.Application.DTOs
{
    public class BlingNotaFiscalServicoResponse
    {
        public List<BlingNotaFiscalServicoData> data { get; set; } = new();
    }

    public class BlingNotaFiscalServicoData
    {
        public long id { get; set; }
        public string numero { get; set; } = string.Empty;
        public string numeroRPS { get; set; } = string.Empty;
        public string serie { get; set; } = string.Empty;
        public int situacao { get; set; }

        // Usamos string? para passar pelo nosso conversor blindado
        public string? dataEmissao { get; set; }

        public decimal valor { get; set; }
        public BlingContatoRef? contato { get; set; }
    }

    public class BlingContatoRef
    {
        public long id { get; set; }
        public string nome { get; set; } = string.Empty;
        public string numeroDocumento { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}