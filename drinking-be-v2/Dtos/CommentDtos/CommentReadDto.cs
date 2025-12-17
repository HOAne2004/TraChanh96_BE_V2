// File: Dtos/CommentDtos/CommentReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.CommentDtos
{
    public class CommentReadDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int NewsId { get; set; }

        public string Content { get; set; } = null!;

        // Thông tin người dùng (Cần Include User)
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? UserThumbnailUrl { get; set; }

        // Trạng thái kiểm duyệt dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        // ⭐ Quan hệ Đệ quy: Danh sách bình luận con (Reply)
        public ICollection<CommentReadDto>? Replies { get; set; }
    }
}