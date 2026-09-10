using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities;

public class NaturezaOperacao
{
    public long Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}