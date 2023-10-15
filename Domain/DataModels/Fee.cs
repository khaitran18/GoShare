using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Fee
    {
        public Fee()
        {
            ChatReceiverNavigations = new HashSet<Chat>();
            ChatSenderNavigations = new HashSet<Chat>();
            Feepolicies = new HashSet<Feepolicy>();
        }

        public Guid Id { get; set; }
        public Guid CarType { get; set; }
        public double BasePrice { get; set; }
        public double BaseDistance { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual Cartype CarTypeNavigation { get; set; } = null!;
        public virtual ICollection<Chat> ChatReceiverNavigations { get; set; }
        public virtual ICollection<Chat> ChatSenderNavigations { get; set; }
        public virtual ICollection<Feepolicy> Feepolicies { get; set; }
    }
}
