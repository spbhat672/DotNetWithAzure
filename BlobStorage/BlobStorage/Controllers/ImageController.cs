using BlobStorage.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly BlobService _blobStorageService;
        public ImageController(BlobService service) {
            _blobStorageService = service;
        }

        [HttpPost("upload")]

        [Consumes("multipart/form-data")]

        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(

 IFormFile file, CancellationToken cancellationToken)

        {

            if (file == null || file.Length == 0)

            {

                return BadRequest("Please select a file.");

            }


            if (file.Length > 10 * 1024 * 1024)

            {

                return BadRequest("Maximum file size is 10 MB.");

            }

            var blobName = await _blobStorageService.UploadAsync(

                file,

                cancellationToken);


            return Ok(new
            {

                Message = "File uploaded successfully",

                OriginalFileName = file.FileName,

                BlobName = blobName,

                FileSize = file.Length

            });

        }

        [HttpGet("download/{fileName}")]

        public async Task<IActionResult> DownloadFile(string fileName)

        {

            var result = await _blobStorageService.DownloadFileAsync(fileName);


            if (result == null)

            {

                return NotFound("File not found in Azure Blob Storage");

            }


            return File(

                result.Value.Content,

                result.Value.ContentType,

                result.Value.FileName

            );

        }
    }
}
