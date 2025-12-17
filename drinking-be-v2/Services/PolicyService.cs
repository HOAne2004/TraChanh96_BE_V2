using AutoMapper;
using drinking_be.Dtos.PolicyDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.PolicyInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Cần dùng SlugGenerator

namespace drinking_be.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PolicyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PolicyReadDto>> GetActivePoliciesAsync(int brandId)
        {
            var repo = _unitOfWork.Repository<Policy>();

            // Lấy Policy đã duyệt của Brand (hoặc Store thuộc Brand đó nếu cần logic phức tạp hơn)
            var policies = await repo.GetAllAsync(
                filter: p => p.BrandId == brandId && p.Status == PolicyReviewStatusEnum.Approved,
                orderBy: q => q.OrderBy(p => p.Title),
                includeProperties: "Brand,Store"
            );

            return _mapper.Map<IEnumerable<PolicyReadDto>>(policies);
        }

        public async Task<PolicyReadDto?> GetPolicyBySlugAsync(string slug)
        {
            var repo = _unitOfWork.Repository<Policy>();

            var policy = await repo.GetFirstOrDefaultAsync(
                filter: p => p.Slug == slug && p.Status == PolicyReviewStatusEnum.Approved,
                includeProperties: "Brand,Store"
            );

            return policy == null ? null : _mapper.Map<PolicyReadDto>(policy);
        }

        public async Task<IEnumerable<PolicyReadDto>> GetAllPoliciesAsync(int? brandId, int? storeId, PolicyReviewStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Policy>();

            var query = await repo.GetAllAsync(
                includeProperties: "Brand,Store",
                orderBy: q => q.OrderByDescending(p => p.CreatedAt)
            );

            if (brandId.HasValue) query = query.Where(p => p.BrandId == brandId.Value);
            if (storeId.HasValue) query = query.Where(p => p.StoreId == storeId.Value);
            if (status.HasValue) query = query.Where(p => p.Status == status.Value);

            return _mapper.Map<IEnumerable<PolicyReadDto>>(query);
        }

        public async Task<PolicyReadDto?> GetByIdAsync(int id)
        {
            var policy = await _unitOfWork.Repository<Policy>().GetFirstOrDefaultAsync(
                filter: p => p.Id == id,
                includeProperties: "Brand,Store"
            );
            return policy == null ? null : _mapper.Map<PolicyReadDto>(policy);
        }

        public async Task<PolicyReadDto> CreatePolicyAsync(PolicyCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Policy>();

            var policy = _mapper.Map<Policy>(dto);
            policy.CreatedAt = DateTime.UtcNow;

            // Tạo Slug tự động từ Title
            policy.Slug = SlugGenerator.GenerateSlug(policy.Title);

            // Kiểm tra trùng Slug (Optional nhưng nên làm)
            var exists = await repo.ExistsAsync(p => p.Slug == policy.Slug);
            if (exists)
            {
                // Thêm hậu tố random nếu trùng
                policy.Slug += $"-{DateTime.UtcNow.Ticks}";
            }

            await repo.AddAsync(policy);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy BrandName
            return (await GetByIdAsync(policy.Id))!;
        }

        public async Task<PolicyReadDto?> UpdatePolicyAsync(int id, PolicyUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Policy>();
            var policy = await repo.GetByIdAsync(id);

            if (policy == null) return null;

            _mapper.Map(dto, policy);

            // Nếu DTO có truyền Slug thì dùng, không thì thôi (không tự tạo lại Slug khi update để tránh gãy link SEO cũ)
            // Nếu muốn tự cập nhật Slug khi đổi Title:
            // if (!string.IsNullOrEmpty(dto.Title)) policy.Slug = SlugGenerator.GenerateSlug(dto.Title);

            policy.UpdatedAt = DateTime.UtcNow;

            repo.Update(policy);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeletePolicyAsync(int id)
        {
            var repo = _unitOfWork.Repository<Policy>();
            var policy = await repo.GetByIdAsync(id);

            if (policy == null) return false;

            // Soft Delete
            policy.Status = PolicyReviewStatusEnum.Deleted;
            policy.DeletedAt = DateTime.UtcNow;

            repo.Update(policy);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}