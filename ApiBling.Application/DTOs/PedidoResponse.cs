using System.Text.Json.Serialization;
using ApiBling.Application.Converters;

namespace ApiBling.Application.DTOs
{
    public class BlingPedidoResponse
    {
        public List<BlingPedidoData> data { get; set; } = new();
    }

    public class BlingPedidoData
    {
        public long id { get; set; }
        public int numero { get; set; }
        public string data { get; set; } = string.Empty;

        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal total { get; set; }
        public BlingPedidoContato contato { get; set; } = new();
        public BlingPedidoSituacao situacao { get; set; } = new();
    }

    public class BlingPedidoContato
    {
        public string nome { get; set; } = string.Empty;
    }

    public class BlingPedidoSituacao
    {
        public int id { get; set; }
    }
}
