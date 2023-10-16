using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record SetPasscodeCommand : IRequest<Task>
    {
        public string Phone { get; set; } = null!;
        public string Passcode { get; set; } = null!;
        public string SetToken { get; set; } = null!;
    }
}
