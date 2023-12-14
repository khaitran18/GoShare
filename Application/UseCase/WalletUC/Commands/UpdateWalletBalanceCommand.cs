using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.WalletUC.Commands
{
    public class UpdateWalletBalanceCommand : IRequest<double>
    {
        public Guid UserId { get; set; }
        public double? Balance { get; set; }
    }
}
