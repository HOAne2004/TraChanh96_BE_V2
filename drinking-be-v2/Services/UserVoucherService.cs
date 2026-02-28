using AutoMapper;
using drinking_be.Dtos.VoucherDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class UserVoucherService : IUserVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserVoucherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserVoucherReadDto>> GetMyVouchersAsync(int userId)
        {
            var repo = _unitOfWork.Repository<UserVoucher>();

            // Lấy voucher chưa dùng, chưa hết hạn, và Template còn active
            var now = DateTime.UtcNow;
            var vouchers = await repo.GetAllAsync(
                filter: uv => uv.UserId == userId
                              && uv.Status == UserVoucherStatusEnum.Unused
                              && uv.ExpiryDate > now,
                includeProperties: "VoucherTemplate",
                orderBy: q => q.OrderBy(v => v.ExpiryDate)
            );

            return _mapper.Map<IEnumerable<UserVoucherReadDto>>(vouchers);
        }

        public async Task<VoucherApplyResultDto> ApplyVoucherAsync(int userId, VoucherApplyDto applyDto)
        {
            var repo = _unitOfWork.Repository<UserVoucher>();

            // 1. Tìm voucher
            var voucher = await repo.GetFirstOrDefaultAsync(
                filter: uv => uv.VoucherCode == applyDto.VoucherCode && uv.UserId == userId,
                includeProperties: "VoucherTemplate"
            );

            // 2. Validate cơ bản
            if (voucher == null)
                return ErrorResult("Mã voucher không tồn tại hoặc không thuộc về bạn.");

            if (voucher.Status != UserVoucherStatusEnum.Unused)
                return ErrorResult("Voucher đã được sử dụng hoặc đã hủy.");

            if (voucher.ExpiryDate < DateTime.UtcNow)
                return ErrorResult("Voucher đã hết hạn.");

            var template = voucher.VoucherTemplate;
            if (template.Status != PublicStatusEnum.Active) // Giả định Template có Status
                return ErrorResult("Chương trình khuyến mãi này đã tạm ngưng.");

            // 3. Validate điều kiện đơn hàng
            if (template.MinOrderValue.HasValue && applyDto.OrderTotalAmount < template.MinOrderValue.Value)
            {
                return ErrorResult($"Đơn hàng phải tối thiểu {template.MinOrderValue.Value:N0}đ để sử dụng.");
            }

            // 4. Tính toán giảm giá
            decimal discount = 0;

            if (template.DiscountType == VoucherDiscountTypeEnum.FixedAmount) // Giảm tiền mặt
            {
                discount = template.DiscountValue;
            }
            else 
            {
                discount = applyDto.OrderTotalAmount * (template.DiscountValue / 100);

                if (template.MaxDiscountAmount.HasValue && discount > template.MaxDiscountAmount.Value)
                {
                    discount = template.MaxDiscountAmount.Value;
                }
            }

            if (discount > applyDto.OrderTotalAmount) discount = applyDto.OrderTotalAmount;

            return new VoucherApplyResultDto
            {
                IsValid = true,
                Message = "Áp dụng thành công.",
                VoucherCode = voucher.VoucherCode ?? string.Empty,
                DiscountAmount = discount,
                FinalAmount = applyDto.OrderTotalAmount - discount,
                UserVoucherId = voucher.Id
            };
        }

        public async Task<UserVoucherReadDto> IssueVoucherAsync(UserVoucherCreateDto dto)
        {
            var uvRepo = _unitOfWork.Repository<UserVoucher>();
            var templateRepo = _unitOfWork.Repository<VoucherTemplate>();

            var template = await templateRepo.GetByIdAsync(dto.VoucherTemplateId);
            if (template == null) throw new Exception("Mẫu voucher không tồn tại.");

            var userVoucher = new UserVoucher
            {
                UserId = dto.UserId,
                VoucherTemplateId = dto.VoucherTemplateId,
                Status = UserVoucherStatusEnum.Unused,
                IssuedDate = DateTime.UtcNow,

                // Hạn dùng: Lấy theo DTO hoặc theo Template
                ExpiryDate = dto.ExpiryDate ?? DateTime.UtcNow.AddDays(30), // Default logic nếu template k có duration

                // Mã: Tự sinh nếu không có
                VoucherCode = !string.IsNullOrEmpty(dto.VoucherCode)
                    ? dto.VoucherCode
                    : GenerateVoucherCode("VOU") // Hoặc logic lấy từ tên: template.Name.Substring(0, 3).ToUpper() // Giả sử Template có Prefix
            };

            await uvRepo.AddAsync(userVoucher);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để include Template cho DTO
            return (await GetVoucherDetailAsync(userVoucher.Id))!;
        }

        public async Task MarkVoucherUsedAsync(long userVoucherId, long orderId)
        {
            var repo = _unitOfWork.Repository<UserVoucher>();
            var voucher = await repo.GetByIdAsync(userVoucherId);

            if (voucher != null)
            {
                voucher.Status = UserVoucherStatusEnum.Used;
                voucher.UsedDate = DateTime.UtcNow;
                voucher.OrderIdUsed = orderId;

                repo.Update(voucher);
                // Không SaveChanges ở đây nếu hàm này được gọi trong Transaction của OrderService
                // Nhưng nếu gọi rời thì cần Save.
                // Ở đây giả định gọi rời hoặc UnitOfWork sẽ handle commit sau.
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RestoreVoucherAsync(long userVoucherId)
        {
            var repo = _unitOfWork.Repository<UserVoucher>();
            var voucher = await repo.GetByIdAsync(userVoucherId);

            if (voucher != null && voucher.Status == UserVoucherStatusEnum.Used)
            {
                voucher.Status = UserVoucherStatusEnum.Unused;
                voucher.UsedDate = null;
                voucher.OrderIdUsed = null;

                repo.Update(voucher);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        // --- Helpers ---
        private VoucherApplyResultDto ErrorResult(string msg) => new VoucherApplyResultDto { IsValid = false, Message = msg };

        private string GenerateVoucherCode(string prefix)
        {
            return $"{prefix}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        private async Task<UserVoucherReadDto?> GetVoucherDetailAsync(long id)
        {
            var voucher = await _unitOfWork.Repository<UserVoucher>().GetFirstOrDefaultAsync(
                v => v.Id == id,
                includeProperties: "VoucherTemplate"
            );
            return _mapper.Map<UserVoucherReadDto>(voucher);
        }
    }
}