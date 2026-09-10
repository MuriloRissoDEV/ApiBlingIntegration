using System.Collections.Generic;

namespace ApiBling.Application.DTOs
{
    public class BlingNotaFiscalResponse
    {
        public List<BlingNotaFiscalData> data { get; set; } = new();
        
    }

    public class BlingNotaFiscalData
    {
        public long id { get; set; }
        public string numero { get; set; } = string.Empty;
        public string dataEmissao { get; set; } = string.Empty;
        public string dataOperacao { get; set; } = string.Empty;
        public BlingContato? contato { get; set; }
        public BlingLoja? loja { get; set; }
        public BlingNaturezaOperacao? naturezaOperacao { get; set; }
    }

    public class BlingContato { public string nome { get; set; } = string.Empty; }
    public class BlingLoja { public long id { get; set; } }
    public class BlingNaturezaOperacao { public long id { get; set; } }



  
    }