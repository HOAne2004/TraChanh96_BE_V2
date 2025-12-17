using drinking_be.Dtos.SizeDtos;

namespace drinking_be.Interfaces.OptionInterfaces
{
    public interface ISizeService
    {
        // Lấy tất cả (Admin xem hết, Client lọc Active)
        Task<IEnumerable<SizeReadDto>> GetAllAsync(bool activeOnly = true);

        Task<SizeReadDto?> GetByIdAsync(short id);

        Task<SizeReadDto> CreateAsync(SizeCreateDto dto);

        Task<SizeReadDto?> UpdateAsync(short id, SizeUpdateDto dto);

        // Xóa mềm
        Task<bool> DeleteAsync(short id);

        // Đếm số sản phẩm đang áp dụng size này (Để cảnh báo trước khi xóa)
        Task<int> CountProductsUsingSizeAsync(short id);
    }
}