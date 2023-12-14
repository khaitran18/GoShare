using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.FeeUC.Command
{
    public record UpdateFeeCommand : IRequest<bool>
    {
        public Guid Guid { get; set; }
        public double base_price { get; set; }
        public double base_distance { get; set; }
    }
}
