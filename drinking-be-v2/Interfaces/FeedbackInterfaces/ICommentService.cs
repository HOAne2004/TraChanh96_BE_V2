using drinking_be.Dtos.CommentDtos;

namespace drinking_be.Interfaces.FeedbackInterfaces
{
    public interface ICommentService
    {
        // Add currentUserId (nullable) to check if the user has liked the comment
        Task<IEnumerable<CommentReadDto>> GetCommentsByNewsIdAsync(int newsId, int? currentUserId);

        Task<CommentReadDto> CreateCommentAsync(int userId, CommentCreateDto dto);

        Task<bool> DeleteCommentAsync(int id, int userId, bool isAdmin);

        // New: Toggle Like
        Task<bool> ToggleLikeAsync(int commentId, int userId);
    }
}