﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.WalletUC.Queries
{
    public record GetWalletBalanceQuery : IRequest<double>
    {

    }
}
