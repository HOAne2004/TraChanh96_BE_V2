using AutoMapper;
using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetApprovedReviewsAsync(int productId)
        {
            var repo = _unitOfWork.Repository<Review>();

            // Lấy đánh giá đã duyệt, sắp xếp mới nhất
            var reviews = await repo.GetAllAsync(
                filter: r => r.ProductId == productId && r.Status == ReviewStatusEnum.Approved,
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                includeProperties: "User" // Để lấy tên người đánh giá
            );

            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        public async Task<ReviewReadDto> CreateReviewAsync(int userId, ReviewCreateDto dto)
        {
            var reviewRepo = _unitOfWork.Repository<Review>();
            var productRepo = _unitOfWork.Repository<Product>();

            // 1. Kiểm tra sản phẩm tồn tại
            var productExists = await productRepo.GetByIdAsync(dto.ProductId);
            if (productExists == null) throw new KeyNotFoundException("Sản phẩm không tồn tại.");

            // 2. Kiểm tra User đã đánh giá chưa (Mỗi user 1 review/sản phẩm)
            var alreadyReviewed = await reviewRepo.ExistsAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);
            if (alreadyReviewed)
            {
                throw new Exception("Bạn đã đánh giá sản phẩm này rồi.");
            }

            // 3. (Optional) Kiểm tra User đã mua hàng chưa? 
            // Logic này phức tạp (cần check Order -> OrderItem), tạm thời bỏ qua ở bước này.

            // 4. Map và Lưu
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.Status = ReviewStatusEnum.Pending; // Mặc định chờ duyệt
            review.CreatedAt = DateTime.UtcNow;

            await reviewRepo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Load lại kèm User để trả về DTO đầy đủ
            var createdReview = await reviewRepo.GetFirstOrDefaultAsync(
                r => r.Id == review.Id,
                includeProperties: "User"
            );

            return _mapper.Map<ReviewReadDto>(createdReview);
        }

        public async Task<IEnumerable<ReviewReadDto>> GetAllReviewsAsync(int? productId, ReviewStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Review>();

            var query = await repo.GetAllAsync(
                includeProperties: "User,Product",
                orderBy: q => q.OrderByDescending(r => r.CreatedAt)
            );

            if (productId.HasValue) query = query.Where(r => r.ProductId == productId.Value);
            if (status.HasValue) query = query.Where(r => r.Status == status.Value);

            return _mapper.Map<IEnumerable<ReviewReadDto>>(query);
        }

        public async Task<ReviewReadDto?> UpdateReviewAsync(int id, ReviewUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Review>();
            var review = await repo.GetByIdAsync(id);

            if (review == null) return null;

            // Update Status hoặc AdminResponse
            if (dto.Status.HasValue) review.Status = dto.Status.Value;
            if (dto.AdminResponse != null) review.AdminResponse = dto.AdminResponse;

            review.UpdatedAt = DateTime.UtcNow;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewReadDto>(review);
        }

        public async Task<bool> DeleteReviewAsync(int id, int userId, bool isAdmin)
        {
            var repo = _unitOfWork.Repository<Review>();
            var review = await repo.GetByIdAsync(id);

            if (review == null) return false;

            // Chỉ Admin hoặc Chủ sở hữu mới được xóa
            if (!isAdmin && review.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa đánh giá này.");
            }

            // Soft Delete
            review.Status = ReviewStatusEnum.Deleted;
            review.DeletedAt = DateTime.UtcNow;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}