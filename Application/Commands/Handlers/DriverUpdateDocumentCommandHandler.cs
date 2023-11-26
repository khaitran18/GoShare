using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Domain.Interfaces;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class DriverUpdateDocumentCommandHandler : IRequestHandler<DriverUpdateDocumentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;
        private readonly IFirebaseStorage _firebaseStorage;

        public DriverUpdateDocumentCommandHandler(IUnitOfWork unitOfWork, UserClaims claims, IFirebaseStorage firebaseStorage)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<bool> Handle(DriverUpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            Guid id = (Guid)_claims.id!;
            string path = id.ToString() + "/DriverDocument";
            var list = await _unitOfWork.DriverDocumentRepository.GetByUserIdAsync(id);
            // loop through the list of documents in the db
            foreach (var document in list)
            {
                // find the picture with the same type from the request
                PictureUploadDto? dto = request.List.FirstOrDefault(d => (short)d.type == document.Type);
                if (dto is not null)
                {
                    document.Url = await _firebaseStorage.UploadFileAsync(dto!.pic, path, dto.type.ToString() + "_" + document.Id);
                    document.UpdateTime = DateTimeUtilities.GetDateTimeVnNow();
                }
            }
            await _unitOfWork.Save();
            return true;
        }
    }
}
