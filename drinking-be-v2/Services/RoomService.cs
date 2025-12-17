using AutoMapper;
using drinking_be.Dtos.RoomDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomReadDto>> GetAllAsync(int? storeId, bool activeOnly = true)
        {
            var repo = _unitOfWork.Repository<Room>();

            // Lấy danh sách kèm Store và ShopTables (để đếm số bàn)
            var query = await repo.GetAllAsync(
                includeProperties: "Store,ShopTables",
                orderBy: q => q.OrderBy(r => r.Name)
            );

            // 1. Lọc theo Store
            if (storeId.HasValue)
            {
                query = query.Where(r => r.StoreId == storeId.Value);
            }

            // 2. Lọc Active
            if (activeOnly)
            {
                query = query.Where(r => r.Status == PublicStatusEnum.Active);
            }

            return _mapper.Map<IEnumerable<RoomReadDto>>(query);
        }

        public async Task<RoomReadDto?> GetByIdAsync(int id)
        {
            var repo = _unitOfWork.Repository<Room>();

            var room = await repo.GetFirstOrDefaultAsync(
                filter: r => r.Id == id,
                includeProperties: "Store,ShopTables"
            );

            return room == null ? null : _mapper.Map<RoomReadDto>(room);
        }

        public async Task<RoomReadDto> CreateAsync(RoomCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Room>();
            var storeRepo = _unitOfWork.Repository<Store>();

            // Validate Store
            var storeExists = await storeRepo.GetByIdAsync(dto.StoreId);
            if (storeExists == null) throw new Exception("Cửa hàng không tồn tại.");

            var room = _mapper.Map<Room>(dto);
            room.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(room.Id))!;
        }

        public async Task<RoomReadDto?> UpdateAsync(int id, RoomUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Room>();
            var room = await repo.GetByIdAsync(id);

            if (room == null) return null;

            _mapper.Map(dto, room);
            room.UpdatedAt = DateTime.UtcNow;

            repo.Update(room);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<Room>();
            var room = await repo.GetByIdAsync(id);

            if (room == null) return false;

            // Soft Delete
            room.Status = PublicStatusEnum.Inactive; // Hoặc Deleted
            room.DeletedAt = DateTime.UtcNow;

            repo.Update(room);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}