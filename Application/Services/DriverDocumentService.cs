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
            int numOfDriverLicense = 1;
            int numOfVehicleRegistration = 1;
            int numOfRegistrationCertificate = 1;
            int numOfCarPicture = 3;
            foreach (string type in Enum.GetNames(typeof(DocumentTypeEnumerations)))
            {
                int dem = 0;
                foreach(PictureUploadDto pictureUploadDto in list)
                {
                    string pictureType = pictureUploadDto.type.ToString();
                    if (pictureType.Equals(type)) dem++;
                }
                if (type.Equals(DocumentTypeEnumerations.DriverLicense.ToString())) if (dem != numOfDriverLicense) throw new BadRequestException("Driver license not found");
                if (type.Equals(DocumentTypeEnumerations.VehicleRegistration.ToString())) if (dem != numOfVehicleRegistration) throw new BadRequestException("Vehicle Registration not found");
                if (type.Equals(DocumentTypeEnumerations.RegistrationCertificate.ToString())) if (dem != numOfRegistrationCertificate) throw new BadRequestException("Registratoin Certificate not found");
                if (type.Equals(DocumentTypeEnumerations.CarPicture.ToString())) if (dem != numOfCarPicture) throw new BadRequestException("Not enough Car Picture");
            }
            return await Task.FromResult(true);
        }
    }
}
