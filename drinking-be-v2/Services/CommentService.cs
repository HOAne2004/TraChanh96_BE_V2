using AutoMapper;
using drinking_be.Dtos.CommentDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentReadDto>> GetCommentsByNewsIdAsync(int newsId)
        {
            var commentRepo = _unitOfWork.Repository<Comment>();

            // Lấy danh sách bình luận GỐC (ParentId == null)
            // Kèm theo:
            // 1. User: Để lấy tên/avatar người bình luận
            // 2. InverseParent: Để lấy danh sách trả lời (Replies)
            // 3. InverseParent.User: Để lấy thông tin người trả lời
            var comments = await commentRepo.GetAllAsync(
                filter: c => c.NewsId == newsId && c.ParentId == null && c.Status == ReviewStatusEnum.Approved,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                includeProperties: "User,InverseParent,InverseParent.User"
            );

            // Mẹo: Nếu muốn lọc cả các Reply phải là Approved, ta có thể lọc lại trong bộ nhớ (Client Evaluation)
            // hoặc cấu hình Global Query Filter. Ở đây ta map sang DTO rồi lọc nhẹ.
            var dtos = _mapper.Map<IEnumerable<CommentReadDto>>(comments);

            // Lọc thủ công các Reply chưa Approved (nếu Include lấy dư)
            foreach (var dto in dtos)
            {
                if (dto.Replies != null)
                {
                    dto.Replies = dto.Replies.Where(r => r.Status == ReviewStatusEnum.Approved.ToString()).ToList();
                }
            }

            return dtos;
        }

        public async Task<CommentReadDto> CreateCommentAsync(int userId, CommentCreateDto dto)
        {
            var commentRepo = _unitOfWork.Repository<Comment>();
            var newsRepo = _unitOfWork.Repository<News>();

            // 1. Kiểm tra bài viết tồn tại
            var newsExists = await newsRepo.GetByIdAsync(dto.NewsId);
            if (newsExists == null) throw new KeyNotFoundException("Bài viết không tồn tại.");

            // 2. Kiểm tra ParentId (nếu là reply)
            if (dto.ParentId.HasValue)
            {
                var parent = await commentRepo.GetByIdAsync(dto.ParentId.Value);
                if (parent == null) throw new Exception("Bình luận gốc không tồn tại.");
                if (parent.NewsId != dto.NewsId) throw new Exception("Bình luận trả lời không cùng bài viết.");
                // Có thể chặn reply cấp 2 nếu muốn (chỉ cho phép 1 cấp)
                if (parent.ParentId != null) throw new Exception("Hệ thống chỉ hỗ trợ trả lời 1 cấp.");
            }

            // 3. Map và Tạo mới
            var comment = _mapper.Map<Comment>(dto);
            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.Status = ReviewStatusEnum.Pending; // Mặc định chờ duyệt

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            // Load lại kèm User để trả về DTO hiển thị ngay
            var createdComment = await commentRepo.GetFirstOrDefaultAsync(
                c => c.Id == comment.Id,
                includeProperties: "User"
            );

            return _mapper.Map<CommentReadDto>(createdComment);
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            var commentRepo = _unitOfWork.Repository<Comment>();

            // Tìm comment (Phải đúng chủ sở hữu)
            var comment = await commentRepo.GetFirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null) return false;

            // Soft Delete
            comment.Status = ReviewStatusEnum.Deleted;
            comment.DeletedAt = DateTime.UtcNow;

            commentRepo.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}