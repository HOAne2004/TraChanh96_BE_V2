using drinking_be.Dtos.NewsDtos;
using drinking_be.Enums;
using drinking_be.Dtos.Common;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface INewsService
    {
        // Public: Lấy danh sách tin đã xuất bản (Published)
        Task<IEnumerable<NewsReadDto>> GetPublishedNewsAsync();

        // Public: Lấy chi tiết tin theo Slug (Chỉ Published)
        Task<NewsReadDto?> GetNewsBySlugAsync(string slug);

        // Admin: Lấy tất cả tin (kể cả Draft, Hidden...)
        Task<PagedResult<NewsReadDto>> GetAllNewsAsync(string? search, ContentStatusEnum? status, int pageIndex = 1, int pageSize = 10);

        // Admin: Lấy chi tiết theo ID
        Task<NewsReadDto?> GetNewsByIdAsync(int id);

        // Admin: Tạo mới
        Task<NewsReadDto> CreateNewsAsync(int userId, NewsCreateDto dto);

        // Admin: Cập nhật
        Task<NewsReadDto?> UpdateNewsAsync(int id, NewsUpdateDto dto);

        // Admin: Xóa (Soft Delete)
        Task<bool> DeleteNewsAsync(int id);
    }
}