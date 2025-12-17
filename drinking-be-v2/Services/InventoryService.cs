using AutoMapper;
using drinking_be.Dtos.InventoryDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore; // Cần để dùng Include

namespace drinking_be.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryReadDto>> GetAllAsync(int? storeId, string? search)
        {
            var repo = _unitOfWork.Repository<Inventory>();

            // 1. Lấy dữ liệu kèm Material và Store
            var query = await repo.GetAllAsync(
                includeProperties: "Material,Store",
                orderBy: q => q.OrderBy(i => i.Material.Name)
            );

            // 2. Lọc theo Kho (StoreId)
            // Lưu ý: storeId có thể là null (Kho tổng). 
            // Nếu API truyền storeId cụ thể -> Lọc đúng kho đó.
            // Nếu muốn lấy tất cả -> Cần logic riêng (ở đây giả định truyền gì lọc nấy)
            if (storeId.HasValue)
            {
                query = query.Where(i => i.StoreId == storeId.Value);
            }
            else
            {
                // Nếu không truyền storeId, mặc định lấy Kho Tổng (StoreId == null)
                // Hoặc tùy nghiệp vụ: Muốn xem tất cả thì bỏ dòng này đi
                query = query.Where(i => i.StoreId == null);
            }

            // 3. Tìm kiếm theo tên nguyên liệu
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(i => i.Material.Name.ToLower().Contains(search));
            }

            // 4. Map sang DTO và tính toán logic
            // Vì IsLowStock cần logic tính toán, ta dùng Select của Linq hoặc Map thủ công
            // Ở đây dùng AutoMapper kết hợp logic tùy chỉnh (xem phần MappingProfile bên dưới)
            return _mapper.Map<IEnumerable<InventoryReadDto>>(query);
        }

        public async Task<InventoryReadDto?> GetByIdAsync(long id)
        {
            var inventory = await _unitOfWork.Repository<Inventory>().GetFirstOrDefaultAsync(
                filter: i => i.Id == id,
                includeProperties: "Material,Store"
            );

            return inventory == null ? null : _mapper.Map<InventoryReadDto>(inventory);
        }

        public async Task<InventoryReadDto> CreateAsync(InventoryCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Inventory>();

            // 1. Kiểm tra xem Nguyên liệu này đã có trong Kho này chưa?
            var exists = await repo.GetFirstOrDefaultAsync(i => i.MaterialId == dto.MaterialId && i.StoreId == dto.StoreId);

            if (exists != null)
            {
                throw new Exception("Nguyên liệu này đã tồn tại trong kho. Vui lòng sử dụng chức năng Cập nhật số lượng.");
            }

            // 2. Tạo mới
            var inventory = _mapper.Map<Inventory>(dto);
            inventory.LastUpdated = DateTime.UtcNow;

            await repo.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy thông tin Material hiển thị
            return (await GetByIdAsync(inventory.Id))!;
        }

        public async Task<InventoryReadDto?> UpdateQuantityAsync(long id, InventoryUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Inventory>();
            var inventory = await repo.GetByIdAsync(id);

            if (inventory == null) return null;

            // Cập nhật số lượng
            inventory.Quantity = dto.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            repo.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var repo = _unitOfWork.Repository<Inventory>();
            var inventory = await repo.GetByIdAsync(id);

            if (inventory == null) return false;

            repo.Delete(inventory);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}