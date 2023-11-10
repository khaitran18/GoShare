﻿using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System.Transactions;

namespace Application.Commands.Handlers
{
    public class DriverRegisterCommandHandler : IRequestHandler<DriverRegisterCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly IMediator _mediator;
        private readonly UserClaims _userClaims;
        private readonly IDriverDocumentService _driverDocumentService;
        private readonly ITokenService _tokenService;

        public DriverRegisterCommandHandler(IUnitOfWork unitOfWork, IFirebaseStorage firebaseStorage, IMediator mediator, UserClaims userClaims, IDriverDocumentService driverDocumentService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorage = firebaseStorage;
            _mediator = mediator;
            _userClaims = userClaims;
            _driverDocumentService = driverDocumentService;
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(DriverRegisterCommand request, CancellationToken cancellationToken)
        {
            //Guid id = (Guid)_userClaims.id!;
            Guid id = _tokenService.GetGuid(request.Token!);
            if (await _driverDocumentService.ValidDocuments(request.List))
            {
                AddCarCommand command = new AddCarCommand
                {
                    UserId = id,
                    Capacity = request.Capacity,
                    Car = request.Car
                };
                Guid carGuid = await _mediator.Send(command);
                string path = id.ToString() + "/DriverDocument";
                foreach (var item in request.List)
                {
                    Driverdocument document = new Driverdocument();
                    document.Id = Guid.NewGuid();
                    document.CarId = carGuid;
                    document.Type = (short)item.type;
                    document.Url = await _firebaseStorage.UploadFileAsync(item.pic, path, item.type.ToString() + "_" + document.Id);
                    await _unitOfWork.DriverDocumentRepository.AddAsync(document);
                    await _unitOfWork.Save();
                }
            }
            else
            {
                throw new BadRequestException("Missing document or document is not valid");
            }
            return Task.CompletedTask.IsCompleted;
        }
    }
}
