using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiBling.Application.DTOs
{
    public class BlingCategoriaResponse
    {
        [JsonPropertyName("data")]
        public List<BlingCategoriaDto> Data { get; set; } = new();
    }

    // DTO que representa cada categoria na lista
    public class BlingCategoriaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("idCategoriaPai")]
        public int? IdCategoriaPai { get; set; }
    }
}

