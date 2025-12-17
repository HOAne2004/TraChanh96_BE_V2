using drinking_be.Dtos.MaterialDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface IMaterialService
    {
        // Lấy danh sách (Có tìm kiếm và lọc theo trạng thái Active)
        Task<IEnumerable<MaterialReadDto>> GetAllAsync(string? search, bool? isActive);

        Task<MaterialReadDto?> GetByIdAsync(int id);

        Task<MaterialReadDto> CreateAsync(MaterialCreateDto dto);

        Task<MaterialReadDto?> UpdateAsync(int id, MaterialUpdateDto dto);

        // Xóa mềm (Chuyển IsActive = false)
        Task<bool> DeleteAsync(int id);
    }
}