// File: Enums/OrderStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum OrderStatusEnum : byte
    {
        // Nhóm 1: Trạng thái Khởi tạo & Xác nhận

        [Description("Mới")]
        New = 1, // Đơn hàng vừa được đặt, chờ xử lý

        [Description("Đã xác nhận")]
        Confirmed = 2, // Đã kiểm tra và xác nhận đơn hàng (thường do Manager thực hiện)

        // Nhóm 2: Trạng thái Chuẩn bị & Vận chuyển

        [Description("Đang chuẩn bị")]
        Preparing = 3, // Cửa hàng đang làm đồ uống

        [Description("Sẵn sàng")]
        Ready = 4, // Đơn hàng đã xong, chờ Shipper/Khách đến lấy

        [Description("Đang giao hàng")]
        Delivering = 5, // Shipper đã nhận hàng và đang giao (Chỉ cho Delivery Order)

        // Nhóm 3: Trạng thái Kết thúc

        [Description("Đã hoàn thành")]
        Completed = 6, // Đơn hàng đã giao thành công và thanh toán

        // Nhóm 4: Trạng thái Hủy

        [Description("Đã bị hủy")]
        Cancelled = 7, // Đơn hàng bị hủy bởi Khách hàng hoặc Cửa hàng
    }
}