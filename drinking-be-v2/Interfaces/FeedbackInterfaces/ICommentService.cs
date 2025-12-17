using drinking_be.Dtos.CommentDtos;

namespace drinking_be.Interfaces.FeedbackInterfaces
{
    public interface ICommentService
    {
        // Lấy danh sách bình luận (Chỉ lấy Approved cho khách xem)
        Task<IEnumerable<CommentReadDto>> GetCommentsByNewsIdAsync(int newsId);

        // Gửi bình luận mới
        Task<CommentReadDto> CreateCommentAsync(int userId, CommentCreateDto dto);

        // Xóa bình luận (Soft Delete) - Chỉ chủ sở hữu mới xóa được
        Task<bool> DeleteCommentAsync(int commentId, int userId);
    }
}