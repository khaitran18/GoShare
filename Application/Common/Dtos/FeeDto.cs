using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class FeeDto
    {
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
