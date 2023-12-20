using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Chat
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Time { get; set; }

        public virtual User ReceiverNavigation { get; set; } = null!;
        public virtual User SenderNavigation { get; set; } = null!;
        public virtual Trip Trip { get; set; } = null!;
    }
}
