using Application.Common.Dtos;
using Application.Services.Interfaces;
using Application.UseCase.DriverUC.Queries;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class GetDriverInformationHandler : IRequestHandler<GetDriverInformationQuery, DriverInformationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;
        private readonly IDriverService _driverService;

        public GetDriverInformationHandler(IUnitOfWork unitOfWork, UserClaims claims, IDriverService driverService)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
            _driverService = driverService;
        }

        public async Task<DriverInformationResponse> Handle(GetDriverInformationQuery request, CancellationToken cancellationToken)
        {
            var response = new DriverInformationResponse();
            response.DailyIncome = await _driverService.GetDriverDailyIncome(_unitOfWork, (Guid)_claims.id!);
            (response.Rating, response.RatingNum) = await _driverService.GetDriverRating(_unitOfWork, (Guid)_claims.id!);
            return response;
        }
    }
}
