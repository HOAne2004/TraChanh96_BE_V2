using AutoMapper;
using drinking_be.Dtos.BrandDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BrandReadDto?> GetPrimaryBrandInfoAsync()
        {
            var repo = _unitOfWork.Repository<Brand>();

            // Lấy brand đầu tiên tìm thấy
            // ⭐ QUAN TRỌNG: Include các bảng con để hiển thị Footer đầy đủ
            var brand = await repo.GetFirstOrDefaultAsync(
                filter: b => b.Status == PublicStatusEnum.Active,
                includeProperties: "Policies,SocialMedias"
            );

            // Nếu không tìm thấy Active, thử tìm cái bất kỳ (để Admin config)
            if (brand == null)
            {
                var allBrands = await repo.GetAllAsync();
                brand = allBrands.FirstOrDefault();
            }

            return brand == null ? null : _mapper.Map<BrandReadDto>(brand);
        }

        public async Task<BrandReadDto> CreateBrandAsync(BrandCreateDto brandDto)
        {
            var repo = _unitOfWork.Repository<Brand>();

            // 1. Kiểm tra xem đã có Brand nào chưa? (Hệ thống chỉ nên có 1 Brand chính)
            var existingBrand = await repo.GetFirstOrDefaultAsync(b => true);
            if (existingBrand != null)
            {
                throw new Exception("Hệ thống đã tồn tại thông tin Thương hiệu. Vui lòng sử dụng chức năng Cập nhật.");
            }

            // 2. Map và Tạo mới
            var newBrand = _mapper.Map<Brand>(brandDto);
            newBrand.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(newBrand);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BrandReadDto>(newBrand);
        }

        public async Task<BrandReadDto?> UpdateBrandAsync(int id, BrandUpdateDto brandDto)
        {
            var repo = _unitOfWork.Repository<Brand>();
            var brand = await repo.GetByIdAsync(id);

            if (brand == null) return null;

            _mapper.Map(brandDto, brand);

            brand.UpdatedAt = DateTime.UtcNow;

            repo.Update(brand);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BrandReadDto>(brand);
        }
    }
}