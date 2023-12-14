using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Services.Interfaces;
using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DriverDocumentService : IDriverDocumentService
    {
        public async Task<bool> ValidDocuments(List<PictureUploadDto> list)
        {
            int numOfId = 2;
            int numOfDriverLicense = 2;
            int numOfVehicleRegistration = 2;
            int numOfRegistrationCertificate = 1;
            int numOfDriverPicture = 1;
            foreach (string type in Enum.GetNames(typeof(DocumentTypeEnumerations)))
            {
                int dem = 0;
                foreach(PictureUploadDto pictureUploadDto in list)
                {
                    string pictureType = pictureUploadDto.type.ToString();
                    if (pictureType.Equals(type)) dem++;
                }
                if (type.Equals(DocumentTypeEnumerations.Id.ToString())) if (dem != numOfId) throw new BadRequestException("Bạn cần phải có "+numOfId+" hình căn cước");
                if (type.Equals(DocumentTypeEnumerations.DriverLicense.ToString())) if (dem != numOfDriverLicense) throw new BadRequestException("Bạn cần phải có "+numOfDriverLicense+" hình bằng lái");
                if (type.Equals(DocumentTypeEnumerations.VehicleRegistration.ToString())) if (dem != numOfVehicleRegistration) throw new BadRequestException("Bạn cần phải có "+numOfDriverLicense+" hình cà vẹt xe");
                if (type.Equals(DocumentTypeEnumerations.RegistrationCertificate.ToString())) if (dem != numOfRegistrationCertificate) throw new BadRequestException("Bạn cần phải có "+numOfRegistrationCertificate+" hình giấy đăng kiểm");
                if (type.Equals(DocumentTypeEnumerations.DriverPicture.ToString())) if (dem != numOfDriverPicture) throw new BadRequestException("Bạn cần phải có "+numOfDriverPicture+" hình bản thân");
            }
            return await Task.FromResult(true);
        }
    }
}
