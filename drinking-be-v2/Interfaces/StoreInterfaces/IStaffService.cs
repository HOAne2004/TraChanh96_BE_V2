using drinking_be.Dtos.StaffDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IStaffService
    {
        // Lấy danh sách nhân viên (Có thể lọc theo Store, Tên)
        Task<IEnumerable<StaffReadDto>> GetAllAsync(int? storeId, string? search);

        Task<StaffReadDto?> GetByIdAsync(int id);
        Task<StaffReadDto?> GetByUserIdAsync(int userId); // Tìm hồ sơ nhân viên theo User ID

        Task<StaffReadDto> CreateAsync(StaffCreateDto createDto);
        Task<StaffReadDto?> UpdateAsync(int id, StaffUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}