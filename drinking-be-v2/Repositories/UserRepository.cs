//using drinking_be.Data;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Repositories
{
    // Kế thừa GenericRepository từ namespace drinking_be.Repositories
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DBDrinkContext context) : base(context)
        {
        }

        //public async Task<bool> IsUserExistsAsync(string email, string username)
        //{
        //    // Bây giờ dbSet đã có thể truy cập được
        //    return await dbSet.AnyAsync(u =>
        //        u.Email.ToLower() == email.ToLower() ||
        //        u.Username.ToLower() == username.ToLower());
        //}
    }
}