using drinking_be.Dtos.AddressDtos;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IAddressService
    {
        // Lấy danh sách địa chỉ của User đang đăng nhập
        Task<IEnumerable<AddressReadDto>> GetAllMyAddressesAsync(int userId);

        // Lấy chi tiết 1 địa chỉ (phải đúng chủ sở hữu)
        Task<AddressReadDto?> GetByIdAsync(long addressId, int userId);

        // Tạo mới
        Task<AddressReadDto> CreateAddressAsync(int userId, AddressCreateDto dto);

        // Cập nhật
        Task<AddressReadDto?> UpdateAddressAsync(long addressId, int userId, AddressUpdateDto dto);

        // Xóa (Soft Delete)
        Task<bool> DeleteAddressAsync(long addressId, int userId);

        // Thiết lập mặc định
        Task<bool> SetDefaultAddressAsync(long addressId, int userId);
    }
}