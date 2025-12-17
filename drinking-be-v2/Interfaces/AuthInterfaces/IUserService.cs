using drinking_be.Dtos.UserDtos;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IUserService
    {
        // Chỉ còn các hàm CRUD thông tin User
        Task<UserReadDto?> GetUserByPublicIdAsync(Guid publicId);
        Task<UserReadDto?> UpdateUserByPublicIdAsync(Guid publicId, UserUpdateDto updateDto);
        Task<bool> DeleteUserByPublicIdAsync(Guid publicId);
    }
}