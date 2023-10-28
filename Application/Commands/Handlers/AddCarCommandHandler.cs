using Application.Common.Dtos;
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

namespace Application.Commands.Handlers
{
    public class AddCarCommandHandler : IRequestHandler<AddCarCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddCarCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(AddCarCommand request, CancellationToken cancellationToken)
        {
            Car response = new Car();
            _mapper.Map(request.Car, response);
            response.Id = Guid.NewGuid();
            response.UserId = request.UserId;
            response.TypeId = await _unitOfWork.CartypeRepository.GetGuidByCapacity(request.Capacity);
            response.Status = (short)CarStatusEnumerations.Not_Verified;
            await _unitOfWork.CarRepository.AddAsync(response);
            return response.Id;
        }
    }
}
