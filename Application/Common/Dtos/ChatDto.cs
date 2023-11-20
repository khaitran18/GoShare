using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class ChatDto
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Time { get; set; }
    }
}
