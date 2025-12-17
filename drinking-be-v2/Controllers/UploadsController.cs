// Controllers/UploadsController.cs
using drinking_be.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace drinking_be.Controllers
{
    // Frontend gọi /api/upload
    [Route("api/upload")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadsController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Tải một tệp tin lên server (chỉ hỗ trợ IFormFile).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // ⭐️ SỬA: BỎ [FromForm] để tránh lỗi schema/parameter parsing của Swagger
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn một tệp để tải lên.");
            }

            try
            {
                var publicUrl = await _uploadService.SaveFileAsync(file);
                // Trả về một đối tượng JSON chứa URL
                return Ok(new { url = publicUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server khi tải lên: {ex.Message}");
            }
        }
    }
}