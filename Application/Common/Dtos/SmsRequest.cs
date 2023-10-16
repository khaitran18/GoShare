using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class SmsRequest
    {
        public string[] to { get; set; } = null!;
        public string content { get; set; } = null!;
        public short type { get; set; }
        public string sender { get; set; } = null!;
    }
}
