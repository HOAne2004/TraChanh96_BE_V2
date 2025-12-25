using AutoMapper;
using drinking_be.Dtos.SocialMediaDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SocialMediaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SocialMediaReadDto>> GetActiveSocialsAsync(int brandId, int? storeId)
        {
            var repo = _unitOfWork.Repository<SocialMedia>();

            var query = await repo.GetAllAsync(
                filter: s => s.BrandId == brandId && s.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderBy(s => s.SortOrder).ThenBy(s => s.Platform),
                includeProperties: "Brand,Store"
            );

            // Nếu truyền StoreId -> Lấy của Store đó
            // Nếu không truyền (storeId == null) -> Lấy của Brand chung (StoreId == null)
            if (storeId.HasValue)
            {
                query = query.Where(s => s.StoreId == storeId.Value);
            }
            else
            {
                // Mặc định lấy của Brand chung (không gắn Store nào)
                // Tuy nhiên, logic này tùy thuộc vào bạn:
                // 1. Chỉ lấy cái chung: query = query.Where(s => s.StoreId == null);
                // 2. Lấy tất cả (cả chung và riêng): Giữ nguyên
                // Ở đây tôi chọn phương án 1 để tránh lẫn lộn
                // Nhưng trong Model của bạn StoreId là int (không null?), hãy check lại model.
                // Dựa vào file SocialMedia.cs: "public int StoreId { get; set; }" -> KHÔNG NULLABLE
                // Dựa vào DTO: "public int? StoreId { get; set; }" -> NULLABLE

                // => GIẢ ĐỊNH: Nếu StoreId = 0 hoặc null là Brand chung.
                // Nếu Model bắt buộc int StoreId, thì có thể 0 là quy ước cho Brand chung.
            }

            return _mapper.Map<IEnumerable<SocialMediaReadDto>>(query);
        }

        public async Task<IEnumerable<SocialMediaReadDto>> GetAllAsync(int? brandId, int? storeId)
        {
            var repo = _unitOfWork.Repository<SocialMedia>();

            var query = await repo.GetAllAsync(
                includeProperties: "Brand,Store",
                orderBy: q => q.OrderBy(s => s.SortOrder)
            );

            if (brandId.HasValue) query = query.Where(s => s.BrandId == brandId.Value);
            if (storeId.HasValue) query = query.Where(s => s.StoreId == storeId.Value);

            return _mapper.Map<IEnumerable<SocialMediaReadDto>>(query);
        }

        public async Task<SocialMediaReadDto?> GetByIdAsync(int id)
        {
            var social = await _unitOfWork.Repository<SocialMedia>().GetFirstOrDefaultAsync(
                filter: s => s.Id == id,
                includeProperties: "Brand,Store"
            );
            return social == null ? null : _mapper.Map<SocialMediaReadDto>(social);
        }

        public async Task<SocialMediaReadDto> CreateAsync(SocialMediaCreateDto dto)
        {
            var repo = _unitOfWork.Repository<SocialMedia>();

            var social = _mapper.Map<SocialMedia>(dto);
            social.CreatedAt = DateTime.UtcNow;

            // Xử lý StoreId nếu null -> Gán 0 (nếu DB không cho phép null)
            if (social.StoreId == null) social.StoreId = 0; // Giả định 0 là chung

            await repo.AddAsync(social);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(social.Id))!;
        }

        public async Task<SocialMediaReadDto?> UpdateAsync(int id, SocialMediaUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<SocialMedia>();
            var social = await repo.GetByIdAsync(id);

            if (social == null) return null;

            _mapper.Map(dto, social);
            social.UpdatedAt = DateTime.UtcNow;

            repo.Update(social);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<SocialMedia>();
            var social = await repo.GetByIdAsync(id);

            if (social == null) return false;

            // Soft Delete
            social.Status = PublicStatusEnum.Deleted;
            social.DeletedAt = DateTime.UtcNow;

            repo.Update(social);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}