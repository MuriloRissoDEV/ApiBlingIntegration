using System;
using System.Collections.Generic;

namespace ApiBling.Domain.Entities
{
    /// <summary>
    /// Representa uma categoria de produtos proveniente do Bling.
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }
        public int IdBling { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public int? IdCategoriaPai { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
