using drinking_be.Dtos.AddressDtos;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IAddressService
    {
        // USER
        Task<IEnumerable<UserAddressReadDto>> GetAllMyAddressesAsync(int userId);
        Task<UserAddressReadDto?> GetByIdAsync(long id, int userId);
        Task<UserAddressReadDto> CreateUserAddressAsync(int userId, UserAddressCreateDto dto);
        Task<UserAddressReadDto?> UpdateUserAddressAsync(long id, int userId, UserAddressUpdateDto dto);
        Task<bool> DeleteUserAddressAsync(long id, int userId);
        Task<bool> SetDefaultAddressAsync(long id, int userId);

        // STORE
        Task<UserAddressReadDto> CreateStoreAddressAsync(int storeId, StoreAddressCreateDto dto);
        Task<UserAddressReadDto?> UpdateStoreAddressAsync(long id, int storeId, StoreAddressCreateDto dto);
        Task<bool> DeleteStoreAddressAsync(long id, int storeId);
    }

}