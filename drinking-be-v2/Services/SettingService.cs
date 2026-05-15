using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.Extensions.Caching.Memory;

namespace drinking_be.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public SettingService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        // Hàm lấy giá trị kiểu Int
        public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
        {
            string cacheKey = $"Setting_{key}";

            // 1. Kiểm tra RAM trước
            if (!_cache.TryGetValue(cacheKey, out string? stringValue))
            {
                // 2. Nếu RAM không có, gọi DB
                var setting = await _unitOfWork.Repository<SystemSetting>().GetByIdAsync(key);
                stringValue = setting?.Value;

                // 3. Lưu vào RAM (Cache 1 tiếng, hoặc cho đến khi Admin sửa)
                if (stringValue != null)
                {
                    _cache.Set(cacheKey, stringValue, TimeSpan.FromHours(1));
                }
            }

            // 4. Ép kiểu (Parse) từ chuỗi sang số
            if (int.TryParse(stringValue, out int result)) return result;

            return defaultValue;
        }

        // (Admin Layer) Khi Admin cập nhật cấu hình, phải xóa Cache để hệ thống lấy giá trị mới
        public async Task UpdateSettingAsync(string key, string newValue)
        {
            var setting = await _unitOfWork.Repository<SystemSetting>().GetByIdAsync(key);
            if (setting != null)
            {
                setting.Value = newValue;
                _unitOfWork.Repository<SystemSetting>().Update(setting);
                await _unitOfWork.CompleteAsync();

                // Xóa cache cũ
                _cache.Remove($"Setting_{key}");
            }
        }
    }
}
