namespace drinking_be.Enums
{
    public enum OrderTypeEnum : short
    {
        AtCounter = 1,  // Tại quầy (Uống tại quán hoặc mang về do nhân viên tạo)
        Delivery = 2,   // Giao hàng tận nơi
        Pickup = 3      // Đặt trước qua App, khách đến lấy
    }
}