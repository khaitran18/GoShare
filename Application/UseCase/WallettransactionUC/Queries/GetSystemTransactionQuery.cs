﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.WallettransactionUC.Queries
{
    public class GetSystemTransactionQuery : IRequest<List<WalletTransactionDto>>
    {

    }
}
