namespace drinking_be.Dtos.CommentDtos
{
    public class CommentReadDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int NewsId { get; set; }
        public string Content { get; set; } = null!;

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? UserThumbnailUrl { get; set; }

        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        // 🟢 MỚI
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; } // Người xem hiện tại đã like chưa?
        public int Level { get; set; }

        public ICollection<CommentReadDto>? Replies { get; set; }
    }
}