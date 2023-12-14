using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Services.Interfaces;
using Application.UseCase.AuthUC.Commands;
using Application.UseCase.DriverUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System.Transactions;

namespace Application.UseCase.AuthUC.Handlers
{
    public class DriverRegisterCommandHandler : IRequestHandler<DriverRegisterCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly IMediator _mediator;
        private readonly IDriverDocumentService _driverDocumentService;

        public DriverRegisterCommandHandler(IUnitOfWork unitOfWork, IFirebaseStorage firebaseStorage, IMediator mediator, IDriverDocumentService driverDocumentService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorage = firebaseStorage;
            _mediator = mediator;
            _driverDocumentService = driverDocumentService;
        }

        public async Task<bool> Handle(DriverRegisterCommand request, CancellationToken cancellationToken)
        {
            //var u = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            User? u = await _unitOfWork.UserRepository.GetUserByOtp(request.Otp);
            Guid id;
            if (u is not null) id = u.Id;
            else throw new NotFoundException("User is not found");
            if (await _unitOfWork.UserRepository.IsDependent(id)) throw new UnauthorizedAccessException("Dependent cannot register to be a driver");
            if (await _driverDocumentService.ValidDocuments(request.List))
            {
                if (!await _unitOfWork.CarRepository.CarDupplicated(id))
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
                else throw new BadRequestException("Registration is existed!");
            }
            else throw new BadRequestException("Missing document or document is not valid");
            return Task.CompletedTask.IsCompleted;
        }
    }
}
