using drinking_be.Dtos.UserDtos;

namespace drinking_be.Interfaces.AuthInterfaces // Hoặc Interfaces.AdminInterfaces nếu muốn
{
    public interface IAdminService
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> UpdateUserByPublicIdAsync(Guid publicId, UserUpdateDto updateDto);
        Task<bool> DeleteUserByPublicIdAsync(Guid publicId);
    }
}