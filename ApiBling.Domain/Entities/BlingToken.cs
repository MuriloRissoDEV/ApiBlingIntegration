using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Domain.Entities
{
    public class BlingToken
    {
        public int Id { get; set; } // Chave Primária
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propriedade calculada super útil para o nosso serviço saber se precisa usar o RefreshToken
        public bool IsExpired => DateTime.UtcNow >= CreatedAt.AddSeconds(ExpiresIn - 60); // Margem de segurança de 60 segundos
    }
}