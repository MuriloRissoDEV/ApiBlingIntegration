using System.Collections.Generic;

namespace ApiBling.Domain.Entities
{
    public class SituacaoNotaFiscalServico
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        // Relacionamento reverso (uma situação tem várias notas)
        public ICollection<NotaFiscalServico> NotasFiscais { get; set; } = new List<NotaFiscalServico>();
    }
}
