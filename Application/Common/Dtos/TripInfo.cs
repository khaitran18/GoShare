using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class DependentTripInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid? DependentId { get; set; }
    }
}
