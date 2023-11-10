using Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IDriverDocumentService
    {
        public Task<bool> ValidDocuments(List<PictureUploadDto> list);
    }
}
