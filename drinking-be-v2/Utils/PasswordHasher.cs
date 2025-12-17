// Utils/PasswordHasher.cs

namespace drinking_be.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Tự động thêm salt và hash
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // ⭐️ Thêm try-catch để bắt lỗi "Invalid salt version"
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception)
            {
                // Nếu hash trong DB bị lỗi (không đúng định dạng BCrypt),
                // ta coi như mật khẩu sai (trả về false) thay vì làm sập API.
                return false;
            }
        }
    }
}