﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.SettingUC.Queries
{
    public class GetSettingsQuery : IRequest<List<SettingDto>>
    {

    }
}
