using System;
using System.Collections.Generic;

namespace ApiBling.Domain.Entities
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public long BlingId { get; set; } // Necessário para a API não duplicar dados

        // Campos solicitados
        public long NaturezaOperacaoId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataOperacao { get; set; }
        public long LojaId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;

        public decimal ValorNota { get; set; }
        public decimal ValorFrete { get; set; }
        public bool OptanteSimplesNacional { get; set; }

        // Controle interno
        public bool DetalhesSincronizados { get; set; } = false;

        // Relacionamento com os itens
        public List<NotaFiscalItem> Itens { get; set; } = new();

    }
}