using AutoMapper;
using drinking_be.Dtos.SizeDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OptionInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SizeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SizeReadDto>> GetAllAsync(bool activeOnly = true)
        {
            var repo = _unitOfWork.Repository<Size>();

            var query = await repo.GetAllAsync(
                orderBy: q => q.OrderBy(s => s.PriceModifier) // Sắp xếp theo giá tăng dần (M -> L)
            );

            if (activeOnly)
            {
                query = query.Where(s => s.Status == PublicStatusEnum.Active);
            }

            return _mapper.Map<IEnumerable<SizeReadDto>>(query);
        }

        public async Task<SizeReadDto?> GetByIdAsync(short id)
        {
            var size = await _unitOfWork.Repository<Size>().GetByIdAsync(id);
            return size == null ? null : _mapper.Map<SizeReadDto>(size);
        }

        public async Task<SizeReadDto> CreateAsync(SizeCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Size>();

            var size = _mapper.Map<Size>(dto);
            size.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(size);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SizeReadDto>(size);
        }

        public async Task<SizeReadDto?> UpdateAsync(short id, SizeUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Size>();
            var size = await repo.GetByIdAsync(id);

            if (size == null) return null;

            _mapper.Map(dto, size);
            size.UpdatedAt = DateTime.UtcNow;

            repo.Update(size);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SizeReadDto>(size);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var repo = _unitOfWork.Repository<Size>();
            var size = await repo.GetByIdAsync(id);

            if (size == null) return false;

            // Soft Delete: Chuyển trạng thái sang Hidden/Deleted
            size.Status = PublicStatusEnum.Inactive;
            size.DeletedAt = DateTime.UtcNow;

            repo.Update(size);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> CountProductsUsingSizeAsync(short id)
        {
            // Đếm trong bảng trung gian ProductSize
            var count = await _unitOfWork.Repository<ProductSize>().CountAsync(ps => ps.SizeId == id);
            return count;
        }
    }
}