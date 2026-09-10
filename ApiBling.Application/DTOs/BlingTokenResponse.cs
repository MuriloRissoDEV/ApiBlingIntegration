using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBling.Application.DTOs
{
    public class BlingTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string token_type { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
    }
}
