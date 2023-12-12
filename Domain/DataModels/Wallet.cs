using Domain.Enumerations;
using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Wallet
    {
        public Wallet()
        {
            Wallettransactions = new HashSet<Wallettransaction>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public double Balance { get; set; }
        public WalletStatus Type { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User? User { get; set; } = null!;
        public virtual ICollection<Wallettransaction> Wallettransactions { get; set; }
    }
}