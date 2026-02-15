using AutoMapper;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
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

            // 1. Lấy đơn hàng (Kèm OrderItems để validate)
            var order = await orderRepo.GetFirstOrDefaultAsync(
                filter: o => o.Id == dto.OrderId && o.UserId == userId,
                includeProperties: "OrderItems" 
            );

            if (order == null)
                throw new KeyNotFoundException("Đơn hàng không tồn tại hoặc không thuộc về bạn.");

            // 2. Validate Trạng thái
            if (order.Status != OrderStatusEnum.Completed)
                throw new InvalidOperationException("Bạn chỉ có thể đánh giá khi đơn hàng đã hoàn thành.");

            // 3. VALIDATE SẢN PHẨM CÓ TRONG ĐƠN KHÔNG (Logic mới)
            var hasProduct = order.OrderItems.Any(oi => oi.ProductId == dto.ProductId);
            if (!hasProduct)
            {
                throw new InvalidOperationException("Bạn không thể đánh giá sản phẩm không có trong đơn hàng này.");
            }

            // 4. Validate Duplicate (1 Đơn - 1 Sản phẩm - 1 Review)
            var existingReview = await reviewRepo.GetFirstOrDefaultAsync(
                r => r.OrderId == dto.OrderId && r.ProductId == dto.ProductId
            );

            if (existingReview != null)
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi.");

            // 5. Tạo Review
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.Status = ReviewStatusEnum.Approved; // Hoặc Pending tùy chính sách
            review.CreatedAt = DateTime.UtcNow;
            var moderationResult = ContentModerator.CheckContent(dto.Content);
            if (!moderationResult.IsClean)
            {
                // Nếu vi phạm -> Ẩn luôn & Ghi lý do
                review.Status = ReviewStatusEnum.Rejected; 
                review.AdminResponse = $"[Hệ thống]: Đánh giá bị ẩn tự động. Lý do: {moderationResult.Reason}";
            }
            else
            {
                // Nếu sạch -> Hiện luôn
                review.Status = ReviewStatusEnum.Approved;
            }
            await reviewRepo.AddAsync(review);

            // 6. 🟢 TÍNH LẠI RATING SẢN PHẨM (Trigger Update)
            // Chỉ tính lại Rating nếu review được Approved
            if (review.Status == ReviewStatusEnum.Approved)
            {
                await UpdateProductRatingAsync(dto.ProductId);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewReadDto>(review);
        }

        // Hàm phụ trợ tính lại sao trung bình
        private async Task UpdateProductRatingAsync(int productId)
        {
            var reviews = await _unitOfWork.Repository<Review>()
                .GetAllAsync(r => r.ProductId == productId && r.Status == ReviewStatusEnum.Approved);

            if (reviews.Any())
            {
                double avgRating = reviews.Average(r => r.Rating);
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product != null)
                {
                    product.TotalRating = Math.Round(avgRating, 1); // Làm tròn 1 số thập phân
                    _unitOfWork.Repository<Product>().Update(product);
                }
            }
        }
        // 2. HÀM LẤY REVIEW CHO ADMIN (Full Filter)
        public async Task<PagedResult<ReviewReadDto>> GetReviewsForAdminAsync(ReviewFilterDto filter)
        {
            var query = _unitOfWork.Repository<Review>().GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Include(r => r.Order) // Include Order để hiện mã đơn
                .AsNoTracking();

            // --- Filter Logic ---
            if (filter.StoreId.HasValue)
            {
                query = query.Where(r =>
                    r.Order != null &&
                    r.Order.StoreId == filter.StoreId
                );
            }

            if (filter.ProductId.HasValue)
                query = query.Where(r => r.ProductId == filter.ProductId);
            if (filter.UserPublicId.HasValue)
                query = query.Where(r => r.User.PublicId == filter.UserPublicId);
            if (filter.FromDate.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(filter.FromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(r => r.CreatedAt >= fromUtc);
            }

            if (filter.ToDate.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(
                    filter.ToDate.Value.Date.AddDays(1),
                    DateTimeKind.Utc
                );
                query = query.Where(r => r.CreatedAt < toUtc);
            }

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status);

            if (filter.Rating.HasValue)
                query = query.Where(r => r.Rating == filter.Rating);

            if (filter.HasReply.HasValue)
            {
                if (filter.HasReply.Value)
                    query = query.Where(r => !string.IsNullOrEmpty(r.AdminResponse));
                else
                    query = query.Where(r => string.IsNullOrEmpty(r.AdminResponse));
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                // 1. Chuyển từ khóa tìm kiếm về chữ thường
                var k = filter.Keyword.Trim().ToLower();

                // 2. So sánh: Chuyển dữ liệu trong DB về chữ thường trước khi check Contains
                query = query.Where(r =>
                    (r.Content != null && r.Content.ToLower().Contains(k)) ||
                    (r.User != null && r.User.Username.ToLower().Contains(k)) ||
                    (r.Order != null && r.Order.OrderCode.ToLower().Contains(k)) ||
                    (r.Product != null && r.Product.Name.ToLower().Contains(k))
                );
            }

            // --- Paging & Sorting ---
            int totalRow = await query.CountAsync();

            // Mặc định mới nhất lên đầu
            var items = await query.OrderByDescending(r => r.CreatedAt)
                                   .Skip((filter.PageIndex - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync();

            var dtos = _mapper.Map<List<ReviewReadDto>>(items);

            return new PagedResult<ReviewReadDto>(dtos, totalRow, filter.PageIndex, filter.PageSize);
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
            var orderRepo = _unitOfWork.Repository<Order>();

            // 1. Lấy các đơn hàng Completed của User
            // ⚠️ QUAN TRỌNG: Phải Include OrderItems để biết mua gì
            var orders = await orderRepo.GetAllAsync(
                filter: o => o.UserId == userId && o.Status == OrderStatusEnum.Completed,
                includeProperties: "OrderItems,Reviews"
            );

            foreach (var order in orders)
            {
                // 2. Check xem đơn này có chứa ProductId không?
                var hasProduct = order.OrderItems.Any(oi => oi.ProductId == productId);

                // 3. Check xem đơn này đã review ProductId này chưa?
                var alreadyReviewed = order.Reviews.Any(r => r.ProductId == productId);

                // Nếu có mua VÀ chưa review -> Cho phép
                if (hasProduct && !alreadyReviewed)
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