using AutoMapper;
using drinking_be.Dtos.CommentDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int MAX_LEVEL = 3; // Maximum nesting level (Parent -> Child -> Grandchild)

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 1. GET COMMENTS
        public async Task<IEnumerable<CommentReadDto>> GetCommentsByNewsIdAsync(int newsId, int? currentUserId)
        {
            var commentRepo = _unitOfWork.Repository<Comment>();

            // Get list + Include Likes to check current user status
            var comments = await commentRepo.GetAllAsync(
                filter: c => c.NewsId == newsId && c.Status == ReviewStatusEnum.Approved,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                includeProperties: "User,Likes" // Important: Include Likes
            );

            // Map all to DTO first
            var allDtos = _mapper.Map<List<CommentReadDto>>(comments);

            // Loop to set IsLiked based on CommentLike table
            foreach (var dto in allDtos)
            {
                var entity = comments.First(c => c.Id == dto.Id);

                // Set LikeCount from DB
                dto.LikeCount = entity.LikeCount;

                // Check if current user liked this comment
                if (currentUserId.HasValue)
                {
                    dto.IsLiked = entity.Likes.Any(l => l.UserId == currentUserId.Value);
                }
            }

            // Build Tree Structure (Recursive)
            var rootDtos = allDtos.Where(c => c.ParentId == null).ToList();
            foreach (var root in rootDtos)
            {
                root.Replies = BuildRepliesTree(root.Id, allDtos);
            }

            return rootDtos;
        }

        // Helper for Recursion
        private List<CommentReadDto> BuildRepliesTree(int parentId, List<CommentReadDto> allComments)
        {
            var replies = allComments
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.CreatedAt) // Oldest replies first
                .ToList();

            foreach (var reply in replies)
            {
                reply.Replies = BuildRepliesTree(reply.Id, allComments);
            }
            return replies;
        }

        // 2. CREATE COMMENT
        public async Task<CommentReadDto> CreateCommentAsync(int userId, CommentCreateDto dto)
        {
            var commentRepo = _unitOfWork.Repository<Comment>();

            // Validate News
            var newsExists = await _unitOfWork.Repository<News>().GetByIdAsync(dto.NewsId);
            if (newsExists == null) throw new Exception("Bài viết không tồn tại.");

            int newLevel = 1;

            // Validate Parent & Level Logic
            if (dto.ParentId.HasValue)
            {
                var parent = await commentRepo.GetByIdAsync(dto.ParentId.Value);
                if (parent == null) throw new Exception("Bình luận gốc không tồn tại.");

                // Limit nesting level
                if (parent.Level >= MAX_LEVEL)
                {
                    // Option: Flatten (reply to grandparent) or Block. Here we block for simplicity.
                    throw new Exception($"Hệ thống chỉ hỗ trợ tối đa {MAX_LEVEL} cấp phản hồi.");
                }

                newLevel = parent.Level + 1;
            }

            var comment = _mapper.Map<Comment>(dto);
            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.Status = ReviewStatusEnum.Pending; // Default pending
            comment.Level = newLevel;
            comment.LikeCount = 0;

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            // Reload with User for FE display
            var createdComment = await commentRepo.GetFirstOrDefaultAsync(c => c.Id == comment.Id, includeProperties: "User");
            return _mapper.Map<CommentReadDto>(createdComment);
        }

        // 3. DELETE COMMENT
        public async Task<bool> DeleteCommentAsync(int id, int userId, bool isAdmin)
        {
            var repo = _unitOfWork.Repository<Comment>();
            var comment = await repo.GetByIdAsync(id);

            if (comment == null) return false;

            // Check permission: Admin or Owner
            if (!isAdmin && comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");
            }

            // Hard delete for simplicity (Cascade delete in DBContext handles replies/likes)
            repo.Delete(comment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // 4. TOGGLE LIKE
        public async Task<bool> ToggleLikeAsync(int commentId, int userId)
        {
            var likeRepo = _unitOfWork.Repository<CommentLike>();
            var commentRepo = _unitOfWork.Repository<Comment>();

            var comment = await commentRepo.GetByIdAsync(commentId);
            if (comment == null) throw new KeyNotFoundException("Bình luận không tồn tại.");

            // Check if user already liked
            var existingLike = await likeRepo.GetFirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

            if (existingLike != null)
            {
                // UNLIKE: Remove from table, Decrease count
                likeRepo.Delete(existingLike);
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
            }
            else
            {
                // LIKE: Add to table, Increase count
                var newLike = new CommentLike { CommentId = commentId, UserId = userId };
                await likeRepo.AddAsync(newLike);
                comment.LikeCount++;
            }

            commentRepo.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            return existingLike == null; // Return true if Liked, false if Unliked
        }
    }
}