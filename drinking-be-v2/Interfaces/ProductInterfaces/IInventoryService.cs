using drinking_be.Dtos.InventoryDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface IInventoryService
    {
        // Lấy danh sách tồn kho
        // storeId: null (Lấy kho tổng) hoặc ID cửa hàng
        // search: Tìm theo tên nguyên liệu
        Task<IEnumerable<InventoryReadDto>> GetAllAsync(int? storeId, string? search);

        Task<InventoryReadDto?> GetByIdAsync(long id);

        // Khởi tạo tồn kho (Cho một nguyên liệu tại một kho)
        Task<InventoryReadDto> CreateAsync(InventoryCreateDto dto);

        // Cập nhật số lượng (Kiểm kho)
        Task<InventoryReadDto?> UpdateQuantityAsync(long id, InventoryUpdateDto dto);

        // Xóa (Nếu nguyên liệu ngừng kinh doanh)
        Task<bool> DeleteAsync(long id);
    }
}