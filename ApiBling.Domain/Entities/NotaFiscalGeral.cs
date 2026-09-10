using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities
{
    public class NotaFiscalGeral
    {
        public long Id { get; set; }
        public int Tipo { get; set; }
        public int Situacao { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataOperacao { get; set; }
        public string ChaveAcesso { get; set; } = string.Empty;

        // Relacionamentos vindos do JSON
        public long? IdNaturezaOperacao { get; set; }
        public long? IdLoja { get; set; }

        // ... seus campos existentes ...

        // NOVOS CAMPOS (Cabeçalho extra que vem no detalhe)
        public decimal ValorFrete { get; set; }
        public int? Finalidade { get; set; }
        public int? TipoNota { get; set; }

        // CONTROLE DE SINCRONIZAÇÃO
        public bool DetalhesSincronizados { get; set; } = false;

        // RELACIONAMENTO
        public ICollection<NotaFiscalGeralItem> Itens { get; set; } = new List<NotaFiscalGeralItem>();
        public decimal ValorTotal { get; set; }
    }
}
