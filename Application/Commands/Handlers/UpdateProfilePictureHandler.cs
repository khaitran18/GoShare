using Application.Common.Dtos;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class UpdateProfilePictureHandler : IRequestHandler<UpdateProfilePictureCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly UserClaims _userClaims;

        public UpdateProfilePictureHandler(IUnitOfWork unitOfWork, IFirebaseStorage firebaseStorage,UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorage = firebaseStorage;
            _userClaims = userClaims;
        }

        public async Task<string> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
        {
            Guid id = (Guid)_userClaims.id!;
            string path = id.ToString();
            string filename = id.ToString() + "_avatar";
            string url = await _firebaseStorage.UploadFileAsync(request.Image, path, filename);
            User? u = await _unitOfWork.UserRepository.GetUserById(id.ToString());
            if (u != null)
            {
                u.AvatarUrl = url;
                await _unitOfWork.UserRepository.UpdateAsync(u);
                await _unitOfWork.Save();
            }
            return url;
        }
    }
}
