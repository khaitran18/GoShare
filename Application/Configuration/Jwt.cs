using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configuration
{
    public class Jwt
    {
        public string key { get; set; } = null!;
        public string expiryTime { get; set; } = null!;
        public string refreshTokenExpiryTime { get; set; } = null!;
        public string issuer { get; set; } = null!;
        public string audience { get; set; } = null!;

    }
}
