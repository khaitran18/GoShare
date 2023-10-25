﻿using Application.Services.Interfaces;
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
            string path = "avatar/";
            Guid.TryParse(_tokenService.ValidateToken(request.Token!)!.FindFirst("id")!.Value, out Guid id);
            return await _firebaseStorage.UploadFileAsync(request.Image, path+id.ToString(),id.ToString());
        }
    }
}
