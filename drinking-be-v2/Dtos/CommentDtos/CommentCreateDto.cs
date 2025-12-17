// File: Dtos/CommentDtos/CommentCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.CommentDtos
{
    public class CommentCreateDto
    {
        // UserId sẽ được Service Layer lấy từ token (Ignore trong mapping)
        // public int UserId { get; set; } 

        [Required(ErrorMessage = "Mã bài viết không được để trống.")]
        public int NewsId { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        // ParentId cho chức năng Reply (Bình luận con)
        public int? ParentId { get; set; }
    }
}