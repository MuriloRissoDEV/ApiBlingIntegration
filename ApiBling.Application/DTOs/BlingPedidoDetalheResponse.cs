using System.Text.Json.Serialization;
using ApiBling.Application.Converters;

namespace ApiBling.Application.DTOs
{
    public class BlingPedidoDetalheResponse
    {
        public BlingPedidoDetalheData data { get; set; } = new();
    }

    public class BlingPedidoDetalheData
    {
        public long id { get; set; }

        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal totalProdutos { get; set; }

        public string observacoes { get; set; } = string.Empty;

        public BlingPedidoDesconto? desconto { get; set; }
        public BlingPedidoTransporte? transporte { get; set; }

       
        public List<BlingPedidoItemDetalhe> itens { get; set; } = new();
    }

    public class BlingPedidoDesconto
    {
        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal valor { get; set; }
    }

    public class BlingPedidoTransporte
    {
        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal frete { get; set; }
        public List<BlingPedidoVolumeData>? volumes { get; set; }
    }

    public class BlingPedidoItemDetalhe
    {
        public long id { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;

        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal quantidade { get; set; }

        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal valor { get; set; }
    }

    public class BlingPedidoVolumeData
    {
        public long id { get; set; }
        public string? servico { get; set; }
        public string? codigoRastreamento { get; set; }
    }
}
