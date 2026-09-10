namespace ApiBling.Domain.Entities
{
    public class NotaFiscalItem
    {
        public int Id { get; set; }
        public int NotaFiscalId { get; set; }
        public NotaFiscal NotaFiscal { get; set; } = null!;

        // Campos solicitados
        public string CodigoProduto { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }

    }
}