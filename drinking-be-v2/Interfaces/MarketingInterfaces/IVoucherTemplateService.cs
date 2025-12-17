using drinking_be.Dtos.VoucherDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IVoucherTemplateService
    {
        // Admin: Lấy danh sách (Filter)
        Task<IEnumerable<VoucherTemplateReadDto>> GetAllAsync(string? search, PublicStatusEnum? status);

        // Public/Admin: Lấy chi tiết
        Task<VoucherTemplateReadDto?> GetByIdAsync(int id);

        // Admin: Tạo mới
        Task<VoucherTemplateReadDto> CreateAsync(VoucherTemplateCreateDto dto);

        // Admin: Cập nhật
        Task<VoucherTemplateReadDto?> UpdateAsync(int id, VoucherTemplateUpdateDto dto);

        // Admin: Xóa (Soft Delete)
        Task<bool> DeleteAsync(int id);
    }
}