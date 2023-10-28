using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IFirebaseStorage
    {
        Task<string> UploadFileAsync(IFormFile fileToUpLoad, string path,string fileName);
    }
}
