using System.Collections.Generic;
using System.Text.Json.Serialization;
using ApiBling.Application.Converters;

namespace ApiBling.Application.DTOs
{
    public class BlingNotaFiscalDetalheResponse
    {
        public BlingNotaFiscalDetalheData data { get; set; } = new();
    }

    public class BlingNotaFiscalDetalheData
    {
        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal valorNota { get; set; }

        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal valorFrete { get; set; }

        public BlingTributacao? tributacao { get; set; }
        public List<BlingNotaFiscalItemDetalhe> itens { get; set; } = new();
    }

    public class BlingTributacao
    {
        // O Bling envia true/false ou 1/0 dependendo do endpoint
        public bool optanteSimplesNacional { get; set; }
    }

    public class BlingNotaFiscalItemDetalhe
    {
        public string codigo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;

        [JsonConverter(typeof(DecimalJsonConverter))]
        [JsonPropertyName("quantidade")]
        public decimal Quantidade { get; set; }

        [JsonConverter(typeof(DecimalJsonConverter))]
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonConverter(typeof(DecimalJsonConverter))]
        [JsonPropertyName("valor_total")]  
        public decimal ValorTotal { get; set; }
    }
}