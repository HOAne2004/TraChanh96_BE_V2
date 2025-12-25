using System.ComponentModel; // ⭐ CẦN THIẾT

namespace drinking_be.Enums
{
    public enum PublicStatusEnum : short
    {
        [Description("Chờ xử lý")]
        Pending = 1,

        [Description("Đang hoạt động")]
        Active = 2,

        [Description("Tạm dừng")]
        Inactive = 3,

        [Description("Đã bị xóa")]
        Deleted = 99 // Giá trị cao để dễ dàng lọc
    }
}