using System.Collections.Generic;

namespace ApiBling.Application.DTOs
{
    public class BlingNotaFiscalGeralDetalheResponse
    {
        public BlingNotaFiscalGeralDetalheData data { get; set; } = new();
    }

    public class BlingNotaFiscalGeralDetalheData
    {
        public decimal valorFrete { get; set; }
        public decimal valorNota { get; set; }
        public string? finalidade { get; set; }
        public string? tipoNota { get; set; }
        public List<BlingNotaFiscalGeralItemDto> itens { get; set; } = new();
    }

    public class BlingNotaFiscalGeralItemDto
    {
        public string codigo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public string unidade { get; set; } = string.Empty;
        public decimal quantidade { get; set; }
        public decimal valor { get; set; }
        public string? tipo { get; set; }
    }
}