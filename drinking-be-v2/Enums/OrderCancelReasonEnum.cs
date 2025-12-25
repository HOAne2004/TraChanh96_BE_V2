using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum OrderCancelReasonEnum : short
    {
        // --- NHÓM 1: KHÁCH HÀNG HỦY ---
        [Description("Khách đổi ý / Không muốn mua nữa")]
        CustomerChangedMind = 1,

        [Description("Khách đặt nhầm món / Nhầm địa chỉ")]
        CustomerOrderWrong = 2,

        // --- NHÓM 2: CỬA HÀNG HỦY ---
        [Description("Hết món / Hết nguyên liệu")]
        StoreOutOfStock = 10,

        [Description("Cửa hàng quá tải / Không thể phục vụ")]
        StoreOverloaded = 11,

        [Description("Cửa hàng đóng cửa đột xuất")]
        StoreClosed = 12,

        // --- NHÓM 3: GIAO HÀNG / HỆ THỐNG HỦY ---
        [Description("Không liên lạc được với khách hàng")]
        UnreachableCustomer = 20,

        [Description("Khách hàng bom hàng (Không nhận)")]
        CustomerRefused = 21,

        [Description("Lý do khác")]
        Other = 99
    }
}