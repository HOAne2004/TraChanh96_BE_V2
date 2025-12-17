// File: Enums/ProductStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum ProductStatusEnum : byte
    {
        [Description("Nháp")]
        Draft = 1, // Trạng thái riêng (Đã thay thế Pending)

        [Description("Đang hoạt động")]
        Active = 2, // ⭐ Giá trị số trùng với PublicStatusEnum

        [Description("Tạm dừng")]
        Inactive = 3, // ⭐ Giá trị số trùng với PublicStatusEnum

        [Description("Hết hàng")]
        OutOfStock = 4, // Trạng thái riêng

        [Description("Đã bị xóa")]
        Deleted = 99 // ⭐ Giá trị số trùng với PublicStatusEnum
    }
}