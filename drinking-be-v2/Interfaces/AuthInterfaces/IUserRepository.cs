using drinking_be.Models;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Kiểm tra xem Email hoặc Username đã tồn tại chưa
        //Task<bool> IsUserExistsAsync(string email, string username);
    }
}