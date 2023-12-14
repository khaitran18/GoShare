using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.FeeUC.Command
{
    public record UpdateFeePolicyCommand : IRequest<bool>
    {
        public Guid Guid { get; set; }
        //public double min_distance { get; set; }
        //public double max_distance { get; set; }
        public double price_per_km { get; set; }
    }
}
