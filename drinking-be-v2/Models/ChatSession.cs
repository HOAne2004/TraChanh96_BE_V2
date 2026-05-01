
namespace drinking_be.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Khóa ngoại liên kết với User (nếu người dùng đã đăng nhập)
        // Cho phép null để hỗ trợ khách chưa đăng nhập vẫn chat được
        public int? UserId { get; set; }

        // ID cửa hàng đang chat (để AI lấy đúng Menu)
        public int StoreId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User? User { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}