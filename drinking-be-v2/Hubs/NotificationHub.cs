using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace drinking_be.Hubs
{
    [Authorize] // Chỉ user đăng nhập mới được kết nối
    public class NotificationHub : Hub
    {
        // Có thể thêm các hàm chat/gửi tin ở đây nếu cần.
        // Hiện tại ta chỉ cần Server bắn xuống Client nên để trống cũng được.

        // Ghi đè OnConnectedAsync để Map UserId vào Connection (đã có sẵn trong Claims nếu dùng JWT)
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}