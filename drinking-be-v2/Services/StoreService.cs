using AutoMapper;
using drinking_be.Dtos.StoreDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Cần SlugGenerator

namespace drinking_be.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductStoreProvisionService _productStoreProvisionService;
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper, IProductStoreProvisionService productStoreProvisionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productStoreProvisionService = productStoreProvisionService;
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

        public async Task<StoreReadDto?> GetStoreByIdAsync(int id)
        {
            var repo = _unitOfWork.Repository<Store>();

            var store = await repo.GetFirstOrDefaultAsync(
                filter: s => s.Id == id && s.Status == StoreStatusEnum.Active,
                includeProperties: "Brand,Address,SocialMedias"
            );

            return _mapper.Map<StoreReadDto>(store);
        }

        public async Task<StoreReadDto?> GetStoreBySlugAsync(string slug)
        {
            var repo = _unitOfWork.Repository<Store>();

            // Lấy chi tiết, Include nhiều thông tin hơn (Social, Policy nếu cần)
            var store = await repo.GetFirstOrDefaultAsync(
                filter: s => s.Slug == slug && s.Status == StoreStatusEnum.Active,
                includeProperties: "Brand,Address,SocialMedias"
            );

            return _mapper.Map<StoreReadDto>(store);
        }

        public async Task<IEnumerable<StoreReadDto>> GetAllStoresAsync(string? search, StoreStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Store>();

            // Load base set (includes)
            var stores = await repo.GetAllAsync(
                includeProperties: "Brand,Address",
                orderBy: q => q.OrderByDescending(s => s.CreatedAt)
            );

            // Apply deletion filtering:
            // - If caller asks explicitly for Deleted, include only deleted stores.
            // - Otherwise exclude Deleted stores by default.
            IEnumerable<Store> query = stores;

            if (status.HasValue)
            {
                if (status.Value == StoreStatusEnum.Deleted)
                {
                    query = query.Where(s => s.Status == StoreStatusEnum.Deleted);
                }
                else
                {
                    query = query.Where(s => s.Status == status.Value && s.Status != StoreStatusEnum.Deleted);
                }
            }
            else
            {
                // default: exclude deleted
                query = query.Where(s => s.Status != StoreStatusEnum.Deleted);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var low = search.ToLower();
                query = query.Where(s => s.Name != null && s.Name.ToLower().Contains(low));
            }

            return _mapper.Map<IEnumerable<StoreReadDto>>(query);
        }

        public async Task<StoreReadDto?> GetByIdAsync(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetFirstOrDefaultAsync(
                filter: s => s.Id == id,
                includeProperties: "Brand,Address,SocialMedias"
            );
            return _mapper.Map<StoreReadDto>(store);
        }

        public async Task<StoreReadDto> CreateStoreAsync(StoreCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Store>();
            var brandRepo = _unitOfWork.Repository<Brand>();
            var addressRepo = _unitOfWork.Repository<Address>();

            // 1. Validate Brand
            if (!await brandRepo.ExistsAsync(b => b.Id == dto.BrandId))
                throw new Exception("Thương hiệu không tồn tại.");

            // 2. Validate Address
            if (!dto.AddressId.HasValue)
                throw new Exception("Địa chỉ chưa được tạo.");

            var address = await addressRepo.GetByIdAsync(dto.AddressId.Value);
            if (address == null)
                throw new Exception("Địa chỉ không tồn tại.");

            // 3. Map Store
            var store = _mapper.Map<Store>(dto);
            store.PublicId = Guid.NewGuid();
            store.CreatedAt = DateTime.UtcNow;
            store.Status = StoreStatusEnum.ComingSoon;
            store.AddressId = address.Id; // ⭐ GÁN NGAY TỪ ĐẦU

            // 4. Generate Slug
            var baseSlug = SlugGenerator.GenerateSlug(store.Name);
            store.Slug = await repo.ExistsAsync(s => s.Slug == baseSlug)
                ? $"{baseSlug}-{Guid.NewGuid().ToString()[..4]}"
                : baseSlug;

            // 5. Save Store
            await repo.AddAsync(store);
            await _unitOfWork.SaveChangesAsync();
            await _productStoreProvisionService
                .InitializeProductStoresForNewStoreAsync(store);
            // 6. Update Address ownership
            address.UserId = null;
            address.StoreId = store.Id;
            address.RecipientName = store.Name;

            if (!string.IsNullOrEmpty(store.PhoneNumber))
                address.RecipientPhone = store.PhoneNumber;

            addressRepo.Update(address);
            await _unitOfWork.SaveChangesAsync();

            // 7. Load lại Store đầy đủ
            var created = await GetStoreByIdForAdminAsync(store.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created store.");

            return _mapper.Map<StoreReadDto>(created);
        }

        public async Task<StoreReadDto?> UpdateStoreAsync(int id, StoreUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Store>();
            var store = await repo.GetByIdAsync(id);

            if (store == null) return null;

            // Map update
            _mapper.Map(dto, store);
            store.UpdatedAt = DateTime.UtcNow;

            repo.Update(store);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
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

        // Private helper: get store regardless of status (for controller/admin return after create)
        private async Task<Store?> GetStoreByIdForAdminAsync(int id)
        {
            var repo = _unitOfWork.Repository<Store>();

            // Use GetFirstOrDefaultAsync without filtering by status so newly created ComingSoon stores are returned
            var store = await repo.GetFirstOrDefaultAsync(
                filter: s => s.Id == id,
                includeProperties: "Brand,Address,SocialMedias,ShopTables"
            );

            return store;
        }
    }
}