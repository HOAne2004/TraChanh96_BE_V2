using drinking_be.Dtos.SocialMediaDtos;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface ISocialMediaService
    {
        // Public: Lấy danh sách Social của Brand (hoặc Store cụ thể)
        Task<IEnumerable<SocialMediaReadDto>> GetActiveSocialsAsync(int brandId, int? storeId);

        // Admin: Lấy tất cả (quản lý)
        Task<IEnumerable<SocialMediaReadDto>> GetAllAsync(int? brandId, int? storeId);

        // Admin: Chi tiết
        Task<SocialMediaReadDto?> GetByIdAsync(int id);

        // Admin: Tạo mới
        Task<SocialMediaReadDto> CreateAsync(SocialMediaCreateDto dto);

        // Admin: Cập nhật
        Task<SocialMediaReadDto?> UpdateAsync(int id, SocialMediaUpdateDto dto);

        // Admin: Xóa
        Task<bool> DeleteAsync(int id);
    }
}