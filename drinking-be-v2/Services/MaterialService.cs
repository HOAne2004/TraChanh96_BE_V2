using AutoMapper;
using drinking_be.Dtos.MaterialDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MaterialService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialReadDto>> GetAllAsync(string? search, bool? isActive)
        {
            var repo = _unitOfWork.Repository<Material>();

            // Query cơ bản
            var query = await repo.GetAllAsync(
                orderBy: q => q.OrderByDescending(m => m.CreatedAt)
            );

            // 1. Lọc theo trạng thái (nếu có truyền)
            if (isActive.HasValue)
            {
                query = query.Where(m => m.IsActive == isActive.Value);
            }

            // 2. Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(m => m.Name.ToLower().Contains(search));
            }

            return _mapper.Map<IEnumerable<MaterialReadDto>>(query);
        }

        public async Task<MaterialReadDto?> GetByIdAsync(int id)
        {
            var material = await _unitOfWork.Repository<Material>().GetByIdAsync(id);
            return material == null ? null : _mapper.Map<MaterialReadDto>(material);
        }

        public async Task<MaterialReadDto> CreateAsync(MaterialCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Material>();

            // Map DTO -> Entity
            var material = _mapper.Map<Material>(dto);

            // Khởi tạo các giá trị hệ thống
            material.PublicId = Guid.NewGuid();
            material.CreatedAt = DateTime.UtcNow;

            // Xử lý conversion rate để tránh chia cho 0 (dù DTO đã validate range)
            if (material.ConversionRate <= 0) material.ConversionRate = 1;

            await repo.AddAsync(material);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MaterialReadDto>(material);
        }

        public async Task<MaterialReadDto?> UpdateAsync(int id, MaterialUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Material>();
            var material = await repo.GetByIdAsync(id);

            if (material == null) return null;

            // Map dữ liệu update
            _mapper.Map(dto, material);

            material.UpdatedAt = DateTime.UtcNow;

            repo.Update(material);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MaterialReadDto>(material);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<Material>();
            var material = await repo.GetByIdAsync(id);

            if (material == null) return false;

            // Soft Delete (Ngưng hoạt động)
            // Thay vì xóa khỏi DB, ta set IsActive = false để giữ lịch sử nhập kho
            material.IsActive = false;
            material.DeletedAt = DateTime.UtcNow;

            repo.Update(material);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}