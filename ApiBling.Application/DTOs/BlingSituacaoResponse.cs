using System.Collections.Generic;

namespace ApiBling.Application.DTOs
{
    public class BlingSituacaoResponse
    {
        public List<BlingSituacaoData> data { get; set; } = new();
    }

    public class BlingSituacaoData
    {
        public long id { get; set; }
        public string nome { get; set; } = string.Empty;
        public long idHerdado { get; set; }
        public string cor { get; set; } = string.Empty;
    }
}
