using ApiBling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace ApiBling.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; } // Chave Primária do nosso banco SQL Server
        public long BlingId { get; set; } // ID original que vem do Bling
        public long? IdProdutoPai { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public decimal Preco { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal Estoque { get; set; }
        public string? Tipo { get; set; }
        public string? Situacao { get; set; }
        public string? Formato { get; set; }
        public string? DescricaoCurta { get; set; }
        public string? ImagemURL { get; set; }

        // Adicione estas propriedades na sua classe Produto existente:
        public string Ncm { get; set; } = string.Empty;
        public string Cest { get; set; } = string.Empty;
        public bool DetalhesSincronizados { get; set; } = false;

        // Campo útil para sabermos quando foi a última vez que sincronizamos este produto
        public DateTime DataUltimaSincronizacao { get; set; } = DateTime.UtcNow;

        public int? IdCategoriaBling { get; set; }
    }
}