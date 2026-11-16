using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace BlobStorage.Service
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            // Generate a unique name for the uploaded file
            var extension = Path.GetExtension(
            Path.GetFileName(file.FileName));

            var blobName = $"{Guid.NewGuid()}{extension}";

            // Get reference to the blob
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Read the uploaded file
            await using var stream = file.OpenReadStream();

            // Upload file to Azure
            await blobClient.UploadAsync(
            stream,
        new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream" : file.ContentType
            }
        },
        cancellationToken);

            return blobName;
        }

        public async Task<(Stream Content, string ContentType, string FileName)?>

   DownloadFileAsync(string fileName)

        {

            BlobClient blobClient = _containerClient.GetBlobClient(fileName);


            if (!await blobClient.ExistsAsync())

            {

                return null;

            }


            BlobDownloadStreamingResult result =

                (await blobClient.DownloadStreamingAsync()).Value;


            string contentType =

                result.Details.ContentType ?? "application/octet-stream";


            return (

                result.Content,

                contentType,

                fileName);

        }

    }
}
