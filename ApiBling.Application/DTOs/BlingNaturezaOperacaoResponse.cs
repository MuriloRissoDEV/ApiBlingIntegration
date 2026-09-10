using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiBling.Application.DTOs;

public class BlingNaturezaOperacaoResponse
{
    [JsonPropertyName("data")]
    public List<BlingNaturezaOperacaoData> Data { get; set; } = new();
}

public class BlingNaturezaOperacaoData
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public string? Tipo { get; set; }
}
