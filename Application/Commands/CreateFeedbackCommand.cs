using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CreateFeedbackCommand : IRequest<AppfeedbackDto>
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
