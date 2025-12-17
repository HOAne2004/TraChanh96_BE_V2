using drinking_be.Dtos.SupplyOrderDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface ISupplyOrderService
    {
        // Tạo phiếu nhập (Manager/Admin)
        Task<SupplyOrderReadDto> CreateAsync(int userId, SupplyOrderCreateDto dto);

        // Lấy danh sách (Filter)
        Task<IEnumerable<SupplyOrderReadDto>> GetAllAsync(int? storeId, SupplyOrderStatusEnum? status, DateTime? fromDate, DateTime? toDate);

        // Chi tiết
        Task<SupplyOrderReadDto?> GetByIdAsync(long id);

        // Cập nhật (Duyệt, Nhập kho, Hủy)
        Task<SupplyOrderReadDto?> UpdateAsync(long id, int userId, SupplyOrderUpdateDto dto);
    }
}