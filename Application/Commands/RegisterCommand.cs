using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record RegisterCommand : IRequest<Task>
    {
        public string Phone { get; set; } = null!;
        public string Name { get; set; } = null!;
        public short? Gender { get; set; }
        public DateTime Birth { get; set; }
    }
}
