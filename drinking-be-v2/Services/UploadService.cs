using drinking_be.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace drinking_be.Services
{
    public class UploadService : IUploadService
    {
        private readonly Supabase.Client _supabaseClient;

        // Tên Bucket bạn đã tạo trên Supabase Dashboard
        private const string BUCKET_NAME = "drinking_files";

        public UploadService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subPath = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Tệp tải lên không hợp lệ.");
            }

            try
            {
                // 1. Tạo tên file duy nhất
                var extension = Path.GetExtension(file.FileName);
                // Lưu ý: Supabase thích đường dẫn kiểu "folder/file.jpg"
                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = $"{subPath}/{fileName}"; // Ví dụ: uploads/abc-xyz.jpg

                // 2. Chuyển file thành mảng byte
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // 3. Upload lên Supabase Storage
                // Hàm Upload trả về đường dẫn file nếu thành công
                await _supabaseClient.Storage
                    .From(BUCKET_NAME)
                    .Upload(fileBytes, fullPath);

                // 4. Lấy đường dẫn công khai (Public URL)
                // Đây là đường dẫn HTTPs thật sự (ví dụ: https://supabase.co/.../abc.jpg)
                var publicUrl = _supabaseClient.Storage
                    .From(BUCKET_NAME)
                    .GetPublicUrl(fullPath);

                return publicUrl;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                throw new Exception($"Lỗi upload Supabase: {ex.Message}");
            }
        }
    }
}