using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Commands
{
    public record VerifyCommand : IRequest<string>
    {
        public string Phone { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
}
