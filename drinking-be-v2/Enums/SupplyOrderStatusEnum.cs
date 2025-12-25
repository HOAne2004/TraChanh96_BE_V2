using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum SupplyOrderStatusEnum : short
    {
        [Description("Chờ duyệt")]
        Pending = 1,        // Store gửi yêu cầu, chờ Admin/Kho tổng duyệt

        [Description("Đã duyệt")]
        Approved = 2,       // Admin đã duyệt, chuẩn bị xuất kho

        [Description("Đang giao hàng")]
        Delivering = 3,     // Hàng đang trên đường tới Store

        [Description("Đã nhận hàng")]
        Received = 4,       // Store đã nhận và kiểm hàng -> Cộng tồn kho (Hoàn thành)

        [Description("Đã hủy")]
        Cancelled = 5,      // Hủy bỏ

        [Description("Từ chối")]
        Rejected = 6        // Admin từ chối yêu cầu (VD: Hết hàng, hoặc Store đặt quá nhiều)
    }
}