using drinking_be.Dtos.RoomDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IRoomService
    {
        // Lấy danh sách phòng (Lọc theo Store)
        Task<IEnumerable<RoomReadDto>> GetAllAsync(int? storeId, bool activeOnly = true);

        // Lấy chi tiết
        Task<RoomReadDto?> GetByIdAsync(int id);

        // Tạo mới
        Task<RoomReadDto> CreateAsync(RoomCreateDto dto);

        // Cập nhật
        Task<RoomReadDto?> UpdateAsync(int id, RoomUpdateDto dto);

        // Xóa (Soft Delete)
        Task<bool> DeleteAsync(int id);
    }
}