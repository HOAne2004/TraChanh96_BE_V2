using AutoMapper;
using drinking_be.Dtos.VoucherDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class VoucherTemplateService : IVoucherTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VoucherTemplateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VoucherTemplateReadDto>> GetAllAsync(string? search, PublicStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<VoucherTemplate>();

            var query = await repo.GetAllAsync(
                includeProperties: "Level", // Lấy thông tin Hạng thành viên
                orderBy: q => q.OrderByDescending(v => v.CreatedAt)
            );

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(v => v.Name.ToLower().Contains(search) ||
                                         (v.CouponCode != null && v.CouponCode.ToLower().Contains(search)));
            }

            return _mapper.Map<IEnumerable<VoucherTemplateReadDto>>(query);
        }

        public async Task<VoucherTemplateReadDto?> GetByIdAsync(int id)
        {
            var template = await _unitOfWork.Repository<VoucherTemplate>().GetFirstOrDefaultAsync(
                filter: v => v.Id == id,
                includeProperties: "Level"
            );
            return template == null ? null : _mapper.Map<VoucherTemplateReadDto>(template);
        }

        public async Task<VoucherTemplateReadDto> CreateAsync(VoucherTemplateCreateDto dto)
        {
            var repo = _unitOfWork.Repository<VoucherTemplate>();

            // 1. Validate Thời gian
            if (dto.EndDate <= dto.StartDate)
            {
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // 2. Validate CouponCode (Nếu có nhập thì phải duy nhất)
            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                var exists = await repo.ExistsAsync(v => v.CouponCode == dto.CouponCode);
                if (exists) throw new Exception($"Mã Coupon '{dto.CouponCode}' đã tồn tại.");
            }

            // 3. Map và Lưu
            var template = _mapper.Map<VoucherTemplate>(dto);
            template.CreatedAt = DateTime.UtcNow;
            template.UsedCount = 0; // Khởi tạo

            await repo.AddAsync(template);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(template.Id))!;
        }

        public async Task<VoucherTemplateReadDto?> UpdateAsync(int id, VoucherTemplateUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<VoucherTemplate>();
            var template = await repo.GetByIdAsync(id);

            if (template == null) return null;

            // Validate logic khi update (Thời gian, Code...)
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
            {
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");
            }

            // Nếu đổi CouponCode, check trùng
            if (!string.IsNullOrEmpty(dto.CouponCode) && dto.CouponCode != template.CouponCode)
            {
                var exists = await repo.ExistsAsync(v => v.CouponCode == dto.CouponCode);
                if (exists) throw new Exception($"Mã Coupon '{dto.CouponCode}' đã tồn tại.");
            }

            _mapper.Map(dto, template);
            template.UpdatedAt = DateTime.UtcNow;

            repo.Update(template);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<VoucherTemplate>();
            var template = await repo.GetByIdAsync(id);

            if (template == null) return false;

            // Soft Delete
            template.Status = PublicStatusEnum.Deleted; // Hoặc Deleted
            template.DeletedAt = DateTime.UtcNow;

            repo.Update(template);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}