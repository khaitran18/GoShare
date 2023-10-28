using Application.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;


namespace Application.Services
{
    public class FirebaseStorage : IFirebaseStorage
    {
        private readonly string _bucketName;
        private StorageClient _storageClient;

        public FirebaseStorage(string bucketName, StorageClient storageClient)
        {
            _bucketName = bucketName;
            _storageClient = storageClient;
        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpLoad,string path, string fileName)
        {
            string bucketName = _bucketName;
            using (var memoryStream = new MemoryStream())
            {
                await fileToUpLoad.CopyToAsync(memoryStream);
                var uploadedFile = await _storageClient.UploadObjectAsync(bucketName, path +"/"+ fileName, "image/jpeg", memoryStream);
                string url = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(uploadedFile.Name)}?alt=media";
                return url;
            }
        }
    }
}
