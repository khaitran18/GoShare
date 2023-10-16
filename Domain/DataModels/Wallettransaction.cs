using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Wallettransaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Guid? TripId { get; set; }
        public double Amount { get; set; }
        public short PaymentMethod { get; set; }
        public string? ExternalTransactionId { get; set; }
        public short Status { get; set; }
        public short Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual Trip? Trip { get; set; }
        public virtual Wallet Wallet { get; set; } = null!;
    }
}
