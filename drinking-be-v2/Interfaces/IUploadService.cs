// Interfaces/IUploadService.cs
using Microsoft.AspNetCore.Http; // Cần IFormFile

namespace drinking_be.Interfaces
{
    public interface IUploadService
    {
        // Trả về đường dẫn công khai (ví dụ: "/uploads/image.png")
        Task<string> SaveFileAsync(IFormFile file, string subPath = "uploads");
    }
}