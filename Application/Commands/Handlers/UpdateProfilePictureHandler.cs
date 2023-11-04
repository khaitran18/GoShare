using Application.Services.Interfaces;
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
    public class UpdateProfilePictureHandler : IRequestHandler<UpdateProfilePictureCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly ITokenService _tokenService;

        public UpdateProfilePictureHandler(IUnitOfWork unitOfWork, IFirebaseStorage firebaseStorage, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorage = firebaseStorage;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
        {
            Guid id = _tokenService.GetGuid(request.Token!);
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
