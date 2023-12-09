using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Commands
{
    public class RevokeCommand : IRequest<Task>
    {
        public string id { get; set; } = null!;
    }
}
