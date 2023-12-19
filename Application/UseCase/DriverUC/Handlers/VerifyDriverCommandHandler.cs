using Application.Common.Exceptions;
using Application.Services.Interfaces;
using Application.UseCase.DriverUC.Commands;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class VerifyDriverCommandHandler : IRequestHandler<VerifyDriverCommand, bool>
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyDriverCommandHandler(ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(VerifyDriverCommand request, CancellationToken cancellationToken)
        {
            //bool c = true;
            //c = c && await _unitOfWork.UserRepository.VerifyDriver(request.id);
            //c = c && await _unitOfWork.CarRepository.VerifyCar(request.id, request.verifiedTo);

            //if (c) return true;
            //else throw new Exception("Error in verifying driver");
            bool response = true;
            User? user = await _unitOfWork.UserRepository.GetUserById(request.id.ToString());
            if (user is null) throw new NotFoundException("User is not found");
            else
            {
                Car? c = await _unitOfWork.CarRepository.GetByUserId(request.id);
                if (c is null) throw new NotFoundException("User's car is not found");
                else
                {
                    var listDocs = await _unitOfWork.DriverDocumentRepository.GetByUserIdAsync(user.Id);
                    if (listDocs is null) throw new NotFoundException("User's document is not found");
                    else
                    {
                        string? avtUrl = listDocs!.FirstOrDefault(p => p.Type == (short)DocumentTypeEnumerations.DriverPicture)?.Url;
                        if (avtUrl is null) throw new NotFoundException("Driver picture is not found");
                        else
                        {
                            response = response && await _unitOfWork.UserRepository.VerifyDriver(user.Id,avtUrl);
                            response = response && await _unitOfWork.CarRepository.VerifyCar(c.Id, request.verifiedTo);
                        }
                    }
                }
            }
            if (response)
            {
                await _unitOfWork.Save();
                return response;
            }
            else throw new Exception("Error in verifying driver");
        }
    }
}
