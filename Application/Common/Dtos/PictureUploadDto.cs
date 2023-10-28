using Domain.Enumerations;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Dtos
{
    public class PictureUploadDto
    {
        public IFormFile pic { get; set; } = null!;
        public DocumentTypeEnumerations type { get; set; }
    }
}
