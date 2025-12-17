using drinking_be.Dtos.FranchiseDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IFranchiseService
    {
        // Khách gửi yêu cầu (Không cần Login)
        Task<FranchiseReadDto> CreateRequestAsync(FranchiseCreateDto dto);

        // Admin: Lấy danh sách (Có lọc theo Trạng thái)
        Task<IEnumerable<FranchiseReadDto>> GetAllAsync(FranchiseStatusEnum? status, string? search);

        // Admin: Xem chi tiết
        Task<FranchiseReadDto?> GetByIdAsync(int id);

        // Admin: Cập nhật (Gán người duyệt, đổi trạng thái, ghi chú)
        Task<FranchiseReadDto?> UpdateRequestAsync(int id, FranchiseUpdateDto dto);

        // Admin: Xóa
        Task<bool> DeleteRequestAsync(int id);
    }
}