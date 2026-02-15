using drinking_be.Dtos.Common;
using drinking_be.Dtos.UserDtos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IUserService
    {
        // Chỉ còn các hàm CRUD thông tin User
        Task<PagedResult<UserReadDto>> GetAllAsync(UserFilterDto filter);
        Task<UserReadDto?> GetUserByPublicIdAsync(Guid publicId);
        Task<UserReadDto?> UpdateUserByPublicIdAsync(Guid publicId, UserUpdateDto updateDto);
        Task<bool> DeleteUserByPublicIdAsync(Guid publicId);
    }
}