using drinking_be.Dtos.PolicyDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.PolicyInterfaces
{
    public interface IPolicyService
    {
        // Public: Lấy danh sách chính sách hiển thị (Approved) của Brand
        Task<IEnumerable<PolicyReadDto>> GetActivePoliciesAsync(int brandId, int? storeId);

        // Public: Lấy chi tiết theo Slug (Chỉ lấy Approved)
        Task<PolicyReadDto?> GetPolicyBySlugAsync(string slug);

        // Admin: Lấy tất cả (Filter theo Brand/Store/Status)
        Task<IEnumerable<PolicyReadDto>> GetAllPoliciesAsync(int? brandId, int? storeId, PolicyReviewStatusEnum? status);

        // Admin: Chi tiết theo ID
        Task<PolicyReadDto?> GetByIdAsync(int id);

        // Admin: Tạo mới
        Task<PolicyReadDto> CreatePolicyAsync(PolicyCreateDto dto);

        // Admin: Cập nhật
        Task<PolicyReadDto?> UpdatePolicyAsync(int id, PolicyUpdateDto dto);

        // Admin: Xóa (Soft Delete)
        Task<bool> DeletePolicyAsync(int id);
    }
}