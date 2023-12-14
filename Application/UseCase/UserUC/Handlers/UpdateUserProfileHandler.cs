using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.UserUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;
        private readonly IFirebaseStorage _firebaseStorage;

        public UpdateUserProfileHandler(IUnitOfWork unitOfWork, UserClaims userClaims, IMapper mapper, IFirebaseStorage firebaseStorage)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
            _mapper = mapper;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            user.Name = request.Name;
            user.Gender = request.Gender;
            user.Birth = request.Birth;

            if (request.Image != null)
            {
                string path = user.Id.ToString();
                string filename = user.Id.ToString() + "_avatar";
                string url = await _firebaseStorage.UploadFileAsync(request.Image, path, filename);

                user.AvatarUrl = url;
            }

            user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.Save();

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }
    }
}
