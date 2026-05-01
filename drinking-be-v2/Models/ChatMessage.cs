using System;

namespace drinking_be.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChatSessionId { get; set; }

        // Sẽ lưu "User" hoặc "Model"
        public string Role { get; set; } = null!;

        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ChatSession ChatSession { get; set; } = null!;
    }
}