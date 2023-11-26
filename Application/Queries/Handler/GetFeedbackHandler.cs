using Application.Common.Dtos;
using Application.Common.Exceptions;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetFeedbackHandler : IRequestHandler<GetFeedbackQuery, AppfeedbackDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFeedbackHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppfeedbackDto> Handle(GetFeedbackQuery request, CancellationToken cancellationToken)
        {
            var feedback = await _unitOfWork.AppfeedbackRepository.GetByIdAsync(request.Id);

            if (feedback == null)
            {
                throw new NotFoundException(nameof(Appfeedback), request.Id);
            }

            var feedbackDto = _mapper.Map<AppfeedbackDto>(feedback);

            return feedbackDto;
        }
    }
}
