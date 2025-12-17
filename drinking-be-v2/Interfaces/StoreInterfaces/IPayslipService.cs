using drinking_be.Dtos.PayslipDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IPayslipService
    {
        // Tạo phiếu lương (Tính toán và Lưu)
        Task<PayslipReadDto> GeneratePayslipAsync(PayslipCreateDto createDto);

        // Lấy danh sách (Lọc theo tháng/năm/store)
        Task<IEnumerable<PayslipReadDto>> GetAllAsync(int? storeId, int? month, int? year);

        // Lấy chi tiết
        Task<PayslipReadDto?> GetByIdAsync(long id);

        // Cập nhật (Sửa thưởng phạt thủ công, chốt lương)
        Task<PayslipReadDto?> UpdateAsync(long id, PayslipUpdateDto updateDto);

        // Xóa (Hủy phiếu lương nếu tính sai)
        Task<bool> DeleteAsync(long id);
    }
}