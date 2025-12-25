// Utils/EnumExtensions.cs (Tạo một file mới)
using System.ComponentModel;
using System.Reflection;

namespace drinking_be.Utils
{
    public static class EnumExtensions
    {
        // Hàm mở rộng để lấy chuỗi mô tả tiếng Việt từ Enum
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());

            if (field == null)
            {
                return value.ToString(); // Trả về tên enum nếu không tìm thấy Description
            }

            DescriptionAttribute? attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}