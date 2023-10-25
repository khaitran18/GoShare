using Application.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FirebaseStorage : IFirebaseStorage
    {
        private readonly string _bucketName;
        private readonly string _credential;

        public FirebaseStorage(string bucketName, string credential)
        {
            _bucketName = bucketName;
            _credential = credential;
        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpLoad,string path, string fileName)
        {
            string bucketName = _bucketName;
            using (var memoryStream = new MemoryStream())
            {
                await fileToUpLoad.CopyToAsync(memoryStream);
                StorageClient _storageClient = StorageClient.Create(GoogleCredential.FromFile(_credential));
                var uploadedFile = await _storageClient.UploadObjectAsync(bucketName, path +"/"+ fileName, "image/jpeg", memoryStream);
                string url = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(uploadedFile.Name)}?alt=media&token={uploadedFile.Generation}";
                return url;
            }
        }
    }
}
