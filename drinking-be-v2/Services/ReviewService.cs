using AutoMapper;
using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

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

        // --- PUBLIC ---
        public async Task<IEnumerable<ReviewReadDto>> GetApprovedReviewsAsync(int productId)
        {
            var repo = _unitOfWork.Repository<Review>();
            var reviews = await repo.GetAllAsync(
                filter: r => r.ProductId == productId && r.Status == ReviewStatusEnum.Approved,
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                includeProperties: "User,Product"
            );
            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        // --- USER: TẠO REVIEW (ĐÃ SỬA LOGIC KHÔNG CẦN ORDER DETAIL) ---
        public async Task<ReviewReadDto> CreateReviewAsync(int userId, ReviewCreateDto dto)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var reviewRepo = _unitOfWork.Repository<Review>();

            // 1. Kiểm tra Đơn hàng có tồn tại và thuộc về User này không?
            // 🔴 SỬA: Bỏ includeProperties: "OrderDetails" vì bạn không có
            var order = await orderRepo.GetFirstOrDefaultAsync(
                filter: o => o.Id == dto.OrderId && o.UserId == userId
            );

            if (order == null)
                throw new KeyNotFoundException("Đơn hàng không tồn tại hoặc không thuộc về bạn.");

            // 2. Kiểm tra trạng thái đơn hàng (Phải giao xong mới được review)
            // Lưu ý: Đảm bảo Model Order của bạn có trường Status
            if (order.Status != OrderStatusEnum.Completed)
                throw new InvalidOperationException("Bạn chỉ có thể đánh giá khi đơn hàng đã hoàn thành.");

            // 🔴 BỎ BƯỚC 3 (Check sản phẩm trong đơn) VÌ KHÔNG CÓ ORDER DETAIL
            // var hasProduct = order.OrderDetails.Any(od => od.ProductId == dto.ProductId);
            // if (!hasProduct) throw ...

            // 4. Kiểm tra đã review chưa (Unique Constraint: OrderId + ProductId)
            // Vẫn giữ check này để tránh 1 đơn đánh giá 2 lần cho cùng 1 món
            var existingReview = await reviewRepo.GetFirstOrDefaultAsync(
                r => r.OrderId == dto.OrderId && r.ProductId == dto.ProductId
            );

            if (existingReview != null)
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này trong đơn hàng này rồi.");

            // 5. Tạo Review
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.Status = ReviewStatusEnum.Pending;
            review.CreatedAt = DateTime.UtcNow;
            review.IsEdited = false;

            await reviewRepo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Load lại review kèm thông tin User để trả về
            var createdReview = await reviewRepo.GetFirstOrDefaultAsync(
                r => r.Id == review.Id,
                includeProperties: "User,Product"
            );

            return _mapper.Map<ReviewReadDto>(createdReview);
        }

        // --- USER: SỬA REVIEW ---
        public async Task<ReviewReadDto> UpdateReviewByUserAsync(int id, int userId, ReviewUserEditDto dto)
        {
            var repo = _unitOfWork.Repository<Review>();
            var review = await repo.GetFirstOrDefaultAsync(r => r.Id == id);

            if (review == null) throw new KeyNotFoundException("Đánh giá không tồn tại.");
            if (review.UserId != userId) throw new UnauthorizedAccessException("Bạn không có quyền sửa đánh giá này.");

            if (dto.Rating > 0) review.Rating = dto.Rating;
            if (dto.Content != null) review.Content = dto.Content;
            if (dto.MediaUrl != null) review.MediaUrl = dto.MediaUrl;

            review.IsEdited = true;
            review.UpdatedAt = DateTime.UtcNow;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewReadDto>(review);
        }

        // --- ADMIN: DUYỆT / TRẢ LỜI ---
        public async Task<ReviewReadDto> UpdateReviewByAdminAsync(int id, ReviewAdminUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Review>();
            var review = await repo.GetFirstOrDefaultAsync(r => r.Id == id);

            if (review == null) throw new KeyNotFoundException("Đánh giá không tồn tại.");

            if (dto.Status.HasValue) review.Status = dto.Status.Value;
            if (dto.AdminResponse != null) review.AdminResponse = dto.AdminResponse;

            review.UpdatedAt = DateTime.UtcNow;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewReadDto>(review);
        }

        // --- COMMON: XÓA ---
        public async Task<bool> DeleteReviewAsync(int id, int userId, bool isAdmin)
        {
            var repo = _unitOfWork.Repository<Review>();
            var review = await repo.GetByIdAsync(id);

            if (review == null) return false;

            if (!isAdmin && review.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa đánh giá này.");

            review.DeletedAt = DateTime.UtcNow;
            repo.Update(review);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // --- HELPER CHO FE (ĐÃ SỬA) ---
        public async Task<bool> CanReviewAsync(int userId, int productId)
        {
            // Logic Mới: Chỉ kiểm tra User có đơn hàng thành công nào CHƯA review sản phẩm này không.
            // Do không có OrderDetail, ta chấp nhận mọi đơn hàng hoàn thành đều "có thể" review món này.

            var orderRepo = _unitOfWork.Repository<Order>();

            // 1. Tìm các đơn hàng đã hoàn thành của user
            var completedOrders = await orderRepo.GetAllAsync(
                filter: o => o.UserId == userId && o.Status == OrderStatusEnum.Completed,
                includeProperties: "Reviews" // Cần include Reviews để check
            );

            // 2. Duyệt qua các đơn hàng
            foreach (var order in completedOrders)
            {
                // Nếu trong đơn hàng này, chưa có Review nào cho ProductId này
                // -> Thì cho phép review (Giả định sản phẩm có trong đơn)
                if (order.Reviews == null || !order.Reviews.Any(r => r.ProductId == productId))
                {
                    return true;
                }
            }

            return false;
        }

        // --- ADMIN: LẤY ALL ---
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
    }
}