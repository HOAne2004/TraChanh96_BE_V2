using AutoMapper;
using drinking_be.Dtos.ShopTableDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class ShopTableService : IShopTableService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShopTableService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShopTableReadDto>> GetTablesByStoreAsync(int storeId, int? roomId)
        {
            var repo = _unitOfWork.Repository<ShopTable>();

            // Lấy danh sách bàn active
            // Include Room và Store
            var query = await repo.GetAllAsync(
                filter: t => t.StoreId == storeId && t.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderBy(t => t.Name),
                includeProperties: "Store,Room"
            );

            if (roomId.HasValue)
            {
                query = query.Where(t => t.RoomId == roomId.Value);
            }

            return _mapper.Map<IEnumerable<ShopTableReadDto>>(query);
        }

        public async Task<ShopTableReadDto?> GetTableByIdAsync(int id)
        {
            var repo = _unitOfWork.Repository<ShopTable>();

            // Include:
            // 1. Store/Room: Hiển thị tên
            // 2. MergedWithTable: Hiển thị tên bàn mẹ (nếu bàn này là con)
            // 3. InverseMergedWithTable: Hiển thị danh sách bàn con (nếu bàn này là mẹ)
            var table = await repo.GetFirstOrDefaultAsync(
                filter: t => t.Id == id,
                includeProperties: "Store,Room,MergedWithTable,InverseMergedWithTable"
            );

            return table == null ? null : _mapper.Map<ShopTableReadDto>(table);
        }

        public async Task<ShopTableReadDto> CreateTableAsync(ShopTableCreateDto dto)
        {
            var repo = _unitOfWork.Repository<ShopTable>();

            // 1. Kiểm tra trùng tên trong Store
            var exists = await repo.ExistsAsync(t => t.StoreId == dto.StoreId && t.Name.ToLower() == dto.Name.ToLower());
            if (exists)
            {
                throw new Exception($"Bàn '{dto.Name}' đã tồn tại trong cửa hàng này.");
            }

            // 2. Validate Room (Nếu có)
            if (dto.RoomId.HasValue)
            {
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(dto.RoomId.Value);
                if (room == null || room.StoreId != dto.StoreId)
                    throw new Exception("Phòng/Khu vực không hợp lệ.");
            }

            // 3. Tạo mới
            var table = _mapper.Map<ShopTable>(dto);
            table.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(table);
            await _unitOfWork.SaveChangesAsync();

            return (await GetTableByIdAsync(table.Id))!;
        }

        public async Task<ShopTableReadDto?> UpdateTableAsync(int id, ShopTableUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<ShopTable>();
            var table = await repo.GetByIdAsync(id);

            if (table == null) return null;

            // 1. Logic Gộp bàn (Nếu update MergedWithTableId)
            if (dto.MergedWithTableId.HasValue)
            {
                if (dto.MergedWithTableId == id) throw new Exception("Không thể gộp bàn vào chính nó.");

                var parentTable = await repo.GetByIdAsync(dto.MergedWithTableId.Value);
                if (parentTable == null || parentTable.StoreId != table.StoreId)
                {
                    throw new Exception("Bàn mẹ không hợp lệ (không tồn tại hoặc khác cửa hàng).");
                }
            }

            // 2. Map và Update
            _mapper.Map(dto, table);
            table.UpdatedAt = DateTime.UtcNow;

            repo.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return (await GetTableByIdAsync(id));
        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            var repo = _unitOfWork.Repository<ShopTable>();
            var table = await repo.GetByIdAsync(id);

            if (table == null) return false;

            // Soft Delete
            table.Status = PublicStatusEnum.Inactive;
            table.DeletedAt = DateTime.UtcNow;

            // Nếu bàn này đang là bàn mẹ, cần giải phóng các bàn con (Optional)
            // Hoặc giữ nguyên logic để bàn con vẫn trỏ vào bàn đã xóa (tùy nghiệp vụ)

            repo.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}