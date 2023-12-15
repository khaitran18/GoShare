using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.SettingUC.Commands
{
    public class UpdateSettingCommand : IRequest<SettingDto>
    {
        public Guid Id { get; set; }
        public double? Value { get; set; }
    }
}
