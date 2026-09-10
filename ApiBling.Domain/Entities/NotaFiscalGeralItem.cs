namespace ApiBling.Domain.Entities
{
    public class NotaFiscalGeralItem
    {
        public long Id { get; set; } // Chave Primária interna
        public long IdNotaFiscalGeral { get; set; } // Chave Estrangeira
        public NotaFiscalGeral NotaFiscalGeral { get; set; } = null!;

        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Tipo { get; set; }
    }
}