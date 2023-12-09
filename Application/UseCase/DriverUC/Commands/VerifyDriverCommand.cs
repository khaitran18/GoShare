using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Commands
{
    public record VerifyDriverCommand : IRequest<bool>
    {
        public Guid id { get; set; }
        public DateTime verifiedTo { get; set; }
    }
}
