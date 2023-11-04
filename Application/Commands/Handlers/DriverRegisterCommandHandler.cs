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
        private readonly ITokenService _tokenService;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly IMediator _mediator;

        public DriverRegisterCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IFirebaseStorage firebaseStorage, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _firebaseStorage = firebaseStorage;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DriverRegisterCommand request, CancellationToken cancellationToken)
        {
            Guid id = _tokenService.GetGuid(request.Token!);
            AddCarCommand command = new AddCarCommand
            {
                UserId = id,
                Capacity = request.Capacity,
                Car = request.Car
            };
            Guid carGuid = await _mediator.Send(command);
            string path = id.ToString()+"/DriverDocument";
            foreach (var item in request.List)
            {
                Driverdocument document = new Driverdocument();
                document.Id = Guid.NewGuid();
                document.CarId = carGuid;
                document.Url = await _firebaseStorage.UploadFileAsync(item.pic, path, item.type.ToString()+"_"+document.Id);
                await _unitOfWork.DriverDocumentRepository.AddAsync(document);
                await _unitOfWork.Save();
            }
            return Task.CompletedTask.IsCompleted;
        }
    }
}
