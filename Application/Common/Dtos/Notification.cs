using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class Notification
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
