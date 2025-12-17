// Interfaces/IJwtGenerator.cs
using drinking_be.Models;
using System.Threading.Tasks; // Cần thiết nếu có các phương thức async khác

namespace drinking_be.Interfaces
{
    public interface IJwtGenerator
    {
        // Tạo token cho User
        string CreateToken(User user);
    }
}