using drinking_be.Dtos.StoreDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IStoreService
    {
        // Public: Lấy danh sách Store đang hoạt động
        Task<IEnumerable<StoreReadDto>> GetActiveStoresAsync();

        //Public: Lấy danh sách Store theo Id
        Task<StoreReadDto?> GetStoreByIdAsync(int id);

        // Public: Lấy chi tiết theo Slug
        Task<StoreReadDto?> GetStoreBySlugAsync(string slug);

        // Admin: Lấy tất cả (Filter)
        Task<IEnumerable<StoreReadDto>> GetAllStoresAsync(string? search, StoreStatusEnum? status);

        // Admin: Chi tiết theo ID
        Task<StoreReadDto?> GetByIdAsync(int id);

        // Admin: Tạo mới
        Task<StoreReadDto> CreateStoreAsync(StoreCreateDto dto);

        // Admin: Cập nhật
        Task<StoreReadDto?> UpdateStoreAsync(int id, StoreUpdateDto dto);

        // Admin: Xóa
        Task<bool> DeleteStoreAsync(int id);
    }
}