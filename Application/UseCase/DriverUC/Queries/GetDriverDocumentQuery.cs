using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Queries
{
    public class GetDriverDocumentQuery : IRequest<List<DriverDocumentDto>>
    {
        public Guid UserId { get; set; }
    }
}
