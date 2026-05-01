using drinking_be.Dtos.ChatDtos;
using drinking_be.Dtos.Common; // Nơi chứa ServiceResponse của bạn

namespace drinking_be.Interfaces
{
    public interface IAIService
    {
        /// <summary>
        /// Gửi tin nhắn đến AI và xử lý nghiệp vụ tự động.
        /// </summary>
        /// <param name="storeId">ID cửa hàng để lấy đúng Menu.</param>
        /// <param name="sessionId">ID phiên chat hiện tại (FE tự generate Guid gửi lên).</param>
        /// <param name="userId">ID người dùng (nếu đã đăng nhập, null nếu là khách).</param>
        /// <param name="userMessage">Nội dung khách hàng chat.</param>
        Task<ServiceResponse<AIChatResponseDto>> SendMessageAsync(int storeId, Guid sessionId, int? userId, string userMessage);
    }
}