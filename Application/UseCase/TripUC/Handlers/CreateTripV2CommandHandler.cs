
using Application.Common.Dtos;
using Application.Services;
using Application.Services.Interfaces;
using Application.UseCase.TripUC.Commands;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class CreateTripV2CommandHandler : IRequestHandler<CreateTripV2Command, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly UserClaims _userClaims;
        private readonly ILogger<BackgroundServices> _logger;

        public CreateTripV2CommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims, ILogger<BackgroundServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
            _logger = logger;
        }

        public Task<string> Handle(CreateTripV2Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
