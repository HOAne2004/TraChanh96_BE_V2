using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.FeedbackInterfaces
{
    public interface IReviewService
    {
        // Public: Lấy đánh giá đã duyệt của sản phẩm
        Task<IEnumerable<ReviewReadDto>> GetApprovedReviewsAsync(int productId);

        // User: Gửi đánh giá mới
        Task<ReviewReadDto> CreateReviewAsync(int userId, ReviewCreateDto dto);

        // Admin: Lấy tất cả (Filter theo Product/Status)
        Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(int? productId, ReviewStatusEnum? status);

        // Admin: Duyệt/Trả lời
        Task<ReviewReadDto?> UpdateReviewAsync(int id, ReviewUpdateDto dto);

        // Admin/User: Xóa
        Task<bool> DeleteReviewAsync(int id, int userId, bool isAdmin);
    }
}