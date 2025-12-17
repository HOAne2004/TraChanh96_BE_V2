// Services/UploadService.cs
using drinking_be.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace drinking_be.Services
{
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subPath = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Tệp tải lên không hợp lệ.");
            }

            // 1. Chuẩn bị đường dẫn vật lý
            // Lấy đường dẫn đến thư mục wwwroot
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, subPath);

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 2. Tạo tên file duy nhất (để tránh xung đột)
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 3. Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. Trả về đường dẫn công khai (URL)
            // Đường dẫn công khai bắt đầu từ gốc web (wwwroot)
            return $"/{subPath}/{fileName}";
        }
    }
}