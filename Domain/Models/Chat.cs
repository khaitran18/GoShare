using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Time { get; set; }

    }
}
