using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Queries
{
    public record GetDriverRegisterCodeQuery : IRequest<string?>
    {
    }
}
