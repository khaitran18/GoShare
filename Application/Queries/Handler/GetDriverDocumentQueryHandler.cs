using Application.Common.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetDriverDocumentQueryHandler : IRequestHandler<GetDriverDocumentQuery, List<DriverDocumentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDriverDocumentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<DriverDocumentDto>> Handle(GetDriverDocumentQuery request, CancellationToken cancellationToken)
        {
            List <DriverDocumentDto> response = new List<DriverDocumentDto>();
            response = _mapper.Map<List<DriverDocumentDto>>(_unitOfWork.DriverDocumentRepository.GetByUserId(request.UserId));
            return Task.FromResult(response);
        }
    }
}
