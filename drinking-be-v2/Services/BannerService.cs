using AutoMapper;
using drinking_be.Dtos.BannerDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerReadDto>> GetActiveBannersAsync(string? position);
        Task<BannerReadDto> CreateAsync(BannerCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class BannerService : IBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BannerReadDto>> GetActiveBannersAsync(string? position)
        {
            var repo = _unitOfWork.Repository<Banner>();
            var query = await repo.GetAllAsync(
                filter: b => b.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderBy(b => b.SortOrder)
            );

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(b => b.Position == position);
            }

            return _mapper.Map<IEnumerable<BannerReadDto>>(query);
        }

        public async Task<BannerReadDto> CreateAsync(BannerCreateDto dto)
        {
            var banner = _mapper.Map<Banner>(dto);
            banner.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Banner>().AddAsync(banner);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BannerReadDto>(banner);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<Banner>();
            var banner = await repo.GetByIdAsync(id);
            if (banner == null) return false;

            repo.Delete(banner); // Xóa cứng hoặc mềm tùy bạn
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}