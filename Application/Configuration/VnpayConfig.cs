using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configuration
{
    public class VnpayConfig
    {
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string CurrCode { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string Locale { get; set; } = null!;
        public string CallBackUrl { get; set; } = null!;
    }
}
