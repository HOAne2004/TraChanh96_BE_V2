using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.FeedbackInterfaces
{
    public interface IReviewService
    {
        // Public
        Task<IEnumerable<ReviewReadDto>> GetApprovedReviewsAsync(int productId);

        // User
        Task<ReviewReadDto> CreateReviewAsync(int userId, ReviewCreateDto dto);
        Task<ReviewReadDto> UpdateReviewByUserAsync(int id, int userId, ReviewUserEditDto dto);
        Task<bool> DeleteReviewAsync(int id, int userId, bool isAdmin);

        // Helper cho FE: Check xem user có quyền review sản phẩm này không (để hiện/ẩn nút)
        Task<bool> CanReviewAsync(int userId, int productId);

        // Admin
        Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(int? productId, ReviewStatusEnum? status);
        Task<ReviewReadDto> UpdateReviewByAdminAsync(int id, ReviewAdminUpdateDto dto);
    }
}