// File: Enums/ProductStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum ProductStatusEnum : short
    {
        [Description("Nháp")]
        Draft = 1, 

        [Description("Đang hoạt động")]
        Active = 2, 

        [Description("Tạm dừng")]
        Inactive = 3, 

        [Description("Hết hàng")]
        OutOfStock = 4, 

        [Description("Đã bị xóa")]
        Deleted = 99 
    }
}