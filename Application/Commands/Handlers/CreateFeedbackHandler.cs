using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, AppfeedbackDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public CreateFeedbackHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<AppfeedbackDto> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var feedback = new Appfeedback
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Content = request.Content,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };

            await _unitOfWork.AppfeedbackRepository.AddAsync(feedback);
            await _unitOfWork.Save();

            var feedbackDto = _mapper.Map<AppfeedbackDto>(feedback);

            return feedbackDto;
        }
    }
}
