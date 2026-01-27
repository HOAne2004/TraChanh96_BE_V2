using AutoMapper;
using drinking_be.Dtos.AddressDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserAddressReadDto>> GetAllMyAddressesAsync(int userId)
        {
            var repo = _unitOfWork.Repository<Address>();

            // Lấy tất cả địa chỉ đang Active của User
            // Sắp xếp: Mặc định lên đầu, sau đó đến mới nhất
            var addresses = await repo.GetAllAsync(
                filter: a => a.UserId == userId && a.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderByDescending(a => a.IsDefault).ThenByDescending(a => a.CreatedAt)
            );

            return _mapper.Map<IEnumerable<UserAddressReadDto>>(addresses);
        }

        public async Task<UserAddressReadDto?> GetByIdAsync(long addressId, int userId)
        {
            var repo = _unitOfWork.Repository<Address>();

            var address = await repo.GetFirstOrDefaultAsync(
                a => a.Id == addressId && a.UserId == userId && a.Status == PublicStatusEnum.Active
            );

            return address == null ? null : _mapper.Map<UserAddressReadDto>(address);
        }

        public async Task<UserAddressReadDto> CreateUserAddressAsync(int userId, UserAddressCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Address>();

            // 1. Logic tự động set Default nếu đây là địa chỉ đầu tiên
            var hasAnyAddress = await repo.ExistsAsync(a => a.UserId == userId && a.Status == PublicStatusEnum.Active);
            if (!hasAnyAddress)
            {
                dto.IsDefault = true;
            }

            // 2. Nếu địa chỉ mới là Default -> Bỏ Default của các địa chỉ cũ
            if (dto.IsDefault)
            {
                await UnsetDefaultOthers(userId);
            }

            // 3. Map DTO -> Entity
            var address = _mapper.Map<Address>(dto);
            address.UserId = userId; // Gán chủ sở hữu
            address.Status = PublicStatusEnum.Active; // Đảm bảo Active
            address.CreatedAt = DateTime.UtcNow;

            // Lưu vào DB
            await repo.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAddressReadDto>(address);
        }

        public async Task<UserAddressReadDto?> UpdateUserAddressAsync(long addressId, int userId, UserAddressUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Address>();

            // Tìm địa chỉ khớp ID và User
            var address = await repo.GetFirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            // Không tìm thấy hoặc đã bị xóa mềm
            if (address == null || address.Status != PublicStatusEnum.Active) return null;

            // Logic: Nếu update set IsDefault = true -> Bỏ Default cũ
            if (dto.IsDefault == true && address.IsDefault != true)
            {
                await UnsetDefaultOthers(userId);
            }

            // Map dữ liệu cập nhật
            _mapper.Map(dto, address);

            address.UpdatedAt = DateTime.UtcNow;

            repo.Update(address);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAddressReadDto>(address);
        }

        public async Task<bool> DeleteUserAddressAsync(long addressId, int userId)
        {
            var repo = _unitOfWork.Repository<Address>();
            var address = await repo.GetFirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null) return false;

            // Soft Delete: Chuyển Status sang Hidden/Deleted
            address.Status = PublicStatusEnum.Deleted;
            address.DeletedAt = DateTime.UtcNow;

            // Nếu xóa địa chỉ mặc định -> Set IsDefault = false (để an toàn)
            if (address.IsDefault == true)
            {
                address.IsDefault = false;
                // Có thể thêm logic tự động chọn địa chỉ khác làm mặc định ở đây nếu muốn
            }

            repo.Update(address);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(long addressId, int userId)
        {
            var repo = _unitOfWork.Repository<Address>();
            var address = await repo.GetFirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null || address.Status != PublicStatusEnum.Active) return false;

            // Bỏ default các cái khác trước
            await UnsetDefaultOthers(userId);

            // Set cái này làm default
            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            repo.Update(address);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // --- Helper Private ---
        private async Task UnsetDefaultOthers(int userId)
        {
            var repo = _unitOfWork.Repository<Address>();
            // Tìm tất cả địa chỉ đang là Default của User này
            var defaults = await repo.GetAllAsync(
                 a => a.UserId == userId
                   && a.IsDefault == true
                   && a.Status == PublicStatusEnum.Active
             );


            foreach (var item in defaults)
            {
                item.IsDefault = false;
                repo.Update(item);
            }
        }

        //====================================
        // Store
        //====================================
        public async Task<UserAddressReadDto> CreateStoreAddressAsync(int storeId, StoreAddressCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Address>();

            var address = _mapper.Map<Address>(dto);
            address.StoreId = storeId; // Gán chủ sở hữu
            address.Status = PublicStatusEnum.Active; // Đảm bảo Active
            address.CreatedAt = DateTime.UtcNow;

            // Lưu vào DB
            await repo.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAddressReadDto>(address);
        }

        public async Task<UserAddressReadDto?> UpdateStoreAddressAsync(long addressId, int storeId, StoreAddressCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Address>();

            // Tìm địa chỉ khớp ID và User
            var address = await repo.GetFirstOrDefaultAsync(a => a.Id == addressId && a.StoreId == storeId);

            // Không tìm thấy hoặc đã bị xóa mềm
            if (address == null || address.Status != PublicStatusEnum.Active) return null;


            // Map dữ liệu cập nhật
            _mapper.Map(dto, address);

            address.UpdatedAt = DateTime.UtcNow;

            repo.Update(address);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserAddressReadDto>(address);
        }

        public async Task<bool> DeleteStoreAddressAsync(long addressId, int storeId)
        {
            var repo = _unitOfWork.Repository<Address>();
            var address = await repo.GetFirstOrDefaultAsync(a => a.Id == addressId && a.StoreId == storeId);

            if (address == null) return false;

            address.Status = PublicStatusEnum.Deleted;
            address.DeletedAt = DateTime.UtcNow;

            repo.Update(address);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}