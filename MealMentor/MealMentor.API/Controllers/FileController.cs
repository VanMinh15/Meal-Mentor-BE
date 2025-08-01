using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealMentor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly string _apkDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "apk");
        private readonly string _apkFileName = "FPTU_MealMentor.apk"; // Fixed file name for the APK

        public FileController()
        {
            // Ensure the directory exists
            if (!Directory.Exists(_apkDirectory))
            {
                Directory.CreateDirectory(_apkDirectory);
            }
        }

        // Endpoint to upload APK file
        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadApk(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is missing.");
                }

                var filePath = Path.Combine(_apkDirectory, _apkFileName);

                // Overwrite the existing file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { Message = "File uploaded successfully", FileName = _apkFileName });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to download APK file
        [HttpGet("download")]
        public IActionResult DownloadApk()
        {
            var filePath = Path.Combine(_apkDirectory, _apkFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.android.package-archive", _apkFileName);
        }
    }
}
