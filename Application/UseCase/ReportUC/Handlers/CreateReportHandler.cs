using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.ReportUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Handlers
{
    public class CreateReportHandler : IRequestHandler<CreateReportCommand, ReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public CreateReportHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            // Check if user is the booker of the trip
            if (trip.BookerId != userId)
            {
                throw new BadRequestException("User is not the booker of this trip.");
            }

            // Check status of the trip (user can only report if trip status is going and completed)
            if (trip.Status != TripStatus.GOING && trip.Status != TripStatus.COMPLETED)
            {
                throw new BadRequestException("User is not allow to report this trip at the moment.");
            }

            // Check if user has already reported this trip
            var existingReport = await _unitOfWork.ReportRepository.GetByTripIdAsync(request.TripId);
            if (existingReport != null)
            {
                throw new BadRequestException("User has already reported this trip.");
            }

            var report = new Report
            {
                Id = Guid.NewGuid(),
                TripId = request.TripId,
                Title = request.Title,
                Description = request.Description,
                Status = ReportStatus.PENDING,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            await _unitOfWork.Save();

            var reportDto = _mapper.Map<ReportDto>(report);

            return reportDto;
        }
    }
}
