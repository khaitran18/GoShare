using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class WalletTransactionDto
    {
        public Guid Id { get; set; }
        public Guid? TripId { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? ExternalTransactionId { get; set; }
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime CreateTime { get; set; }
    }
}
