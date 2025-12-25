namespace drinking_be.Enums
{
    public enum ProductStoreStatusEnum : short
    {
        Disabled = 0,     // Không bán tại cửa hàng này 
        Available = 1,    // Đang bán
        OutOfStock = 2,   // Hết hàng tạm thời
        Hidden = 3        // Ẩn khỏi UI nhưng không xóa
    }

}
