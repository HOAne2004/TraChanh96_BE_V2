using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum OrderStatusEnum : short
    {
        // Nhóm 0: Chờ thanh toán (Dành cho Online Payment)
        [Description("Chờ thanh toán")]
        PendingPayment = 0,

        // Nhóm 1: Trạng thái Khởi tạo & Xác nhận
        [Description("Mới")]
        New = 1, // Đơn hàng mới (COD hoặc đã thanh toán xong), chờ nhân viên xác nhận

        [Description("Đã xác nhận")]
        Confirmed = 2, // Nhân viên đã nhận đơn

        // Nhóm 2: Trạng thái Chuẩn bị & Vận chuyển
        [Description("Đang chuẩn bị")]
        Preparing = 3, // Bếp đang làm

        [Description("Sẵn sàng")]
        Ready = 4, // Đã xong, chờ Shipper đến lấy hoặc Khách tự lấy (Pickup)

        [Description("Đang giao hàng")]
        Delivering = 5, // Shipper đang đi giao

        // Nhóm 3: Trạng thái Kết thúc
        [Description("Đã hoàn thành")]
        Completed = 6, // Giao hàng thành công (Delivery)

        [Description("Đã nhận hàng")]
        Received = 8, // Khách đã lấy đồ tại quầy thành công (AtCounter)

        // Nhóm 4: Trạng thái Hủy
        [Description("Đã hủy")]
        Cancelled = 7, // Đơn bị hủy
    }
}