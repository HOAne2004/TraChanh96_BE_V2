using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface IPaymentMethodService
    {
        // Public: Lấy danh sách phương thức đang hoạt động
        Task<IEnumerable<PaymentMethodReadDto>> GetActiveMethodsAsync();

        // Admin: Lấy tất cả (quản lý)
        Task<IEnumerable<PaymentMethodReadDto>> GetAllMethodsAsync();

        // Admin: Lấy chi tiết
        Task<PaymentMethodReadDto?> GetByIdAsync(int id);

        // Admin: Tạo mới
        Task<PaymentMethodReadDto> CreateAsync(PaymentMethodCreateDto dto);

        // Admin: Cập nhật
        Task<PaymentMethodReadDto?> UpdateAsync(int id, PaymentMethodUpdateDto dto);

        // Admin: Xóa (Soft Delete)
        Task<bool> DeleteAsync(int id);

        Task<bool> ToggleStatusAsync(int id);
    }
}