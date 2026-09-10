using System;

namespace ApiBling.Domain.Entities
{
    public class NotaFiscalServico
    {
        public long Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string NumeroRPS { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;

        public int Situacao { get; set; }
        public SituacaoNotaFiscalServico SituacaoObj { get; set; } = null!;
        public DateTime? DataEmissao { get; set; }
        public decimal Valor { get; set; }

        // Dados do Contato
        public long? IdContato { get; set; }
        public string NomeContato { get; set; } = string.Empty;
        public string DocumentoContato { get; set; } = string.Empty;
        public string EmailContato { get; set; } = string.Empty;
    }
}
