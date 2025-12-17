using AutoMapper;
using drinking_be.Dtos.StoreDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Cần SlugGenerator

namespace drinking_be.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StoreReadDto>> GetActiveStoresAsync()
        {
            var repo = _unitOfWork.Repository<Store>();

            // Lấy Store Active, include Brand và Address để hiển thị Card
            var stores = await repo.GetAllAsync(
                filter: s => s.Status == StoreStatusEnum.Active,
                orderBy: q => q.OrderBy(s => s.SortOrder).ThenBy(s => s.Name),
                includeProperties: "Brand,Address"
            );

            return _mapper.Map<IEnumerable<StoreReadDto>>(stores);
        }

        public async Task<StoreReadDto?> GetStoreBySlugAsync(string slug)
        {
            var repo = _unitOfWork.Repository<Store>();

            // Lấy chi tiết, Include nhiều thông tin hơn (Social, Policy nếu cần)
            var store = await repo.GetFirstOrDefaultAsync(
                filter: s => s.Slug == slug && s.Status == StoreStatusEnum.Active,
                includeProperties: "Brand,Address,SocialMedias"
            );

            return store == null ? null : _mapper.Map<StoreReadDto>(store);
        }

        public async Task<IEnumerable<StoreReadDto>> GetAllStoresAsync(string? search, StoreStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Store>();

            var query = await repo.GetAllAsync(
                includeProperties: "Brand,Address",
                orderBy: q => q.OrderByDescending(s => s.CreatedAt)
            );

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search));
            }

            return _mapper.Map<IEnumerable<StoreReadDto>>(query);
        }

        public async Task<StoreReadDto?> GetByIdAsync(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetFirstOrDefaultAsync(
                filter: s => s.Id == id,
                includeProperties: "Brand,Address,SocialMedias"
            );
            return store == null ? null : _mapper.Map<StoreReadDto>(store);
        }

        public async Task<StoreReadDto> CreateStoreAsync(StoreCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Store>();
            var brandRepo = _unitOfWork.Repository<Brand>();
            var addressRepo = _unitOfWork.Repository<Address>();

            // 1. Validate Brand & Address
            if (!await brandRepo.ExistsAsync(b => b.Id == dto.BrandId))
                throw new Exception("Thương hiệu không tồn tại.");

            if (!await addressRepo.ExistsAsync(a => a.Id == dto.AddressId))
                throw new Exception("Địa chỉ không tồn tại.");

            // 2. Map & Create
            var store = _mapper.Map<Store>(dto);
            store.PublicId = Guid.NewGuid();
            store.CreatedAt = DateTime.UtcNow;

            // 3. Tạo Slug
            string baseSlug = SlugGenerator.GenerateSlug(store.Name);
            store.Slug = baseSlug;

            // Check trùng slug đơn giản (nếu trùng thì thêm random string)
            if (await repo.ExistsAsync(s => s.Slug == baseSlug))
            {
                store.Slug = $"{baseSlug}-{Guid.NewGuid().ToString().Substring(0, 4)}";
            }

            await repo.AddAsync(store);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy thông tin Address/Brand hiển thị
            return (await GetByIdAsync(store.Id))!;
        }

        public async Task<StoreReadDto?> UpdateStoreAsync(int id, StoreUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Store>();
            var store = await repo.GetByIdAsync(id);

            if (store == null) return null;

            // Map update
            _mapper.Map(dto, store);
            store.UpdatedAt = DateTime.UtcNow;

            // Nếu muốn cập nhật Slug khi đổi tên, thêm logic ở đây (thường thì hạn chế đổi slug)

            repo.Update(store);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id));
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {
            var repo = _unitOfWork.Repository<Store>();
            var store = await repo.GetByIdAsync(id);

            if (store == null) return false;

            // Soft Delete
            store.Status = StoreStatusEnum.Deleted;
            store.DeletedAt = DateTime.UtcNow;

            repo.Update(store);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}