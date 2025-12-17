using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum NotificationTypeEnum : byte
    {
        [Description("Hệ thống")]
        System = 1,      // Bảo trì, cập nhật app

        [Description("Đơn hàng")]
        Order = 2,       // Đặt hàng thành công, Đang giao, Đã hủy

        [Description("Khuyến mãi")]
        Promotion = 3,   // Voucher mới, Giảm giá (Thường là hẹn giờ)

        [Description("Cá nhân")]
        Personal = 4     // Tin nhắn riêng từ Admin (Cảnh báo, Chúc mừng sinh nhật)
    }
}