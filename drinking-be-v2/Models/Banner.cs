using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;
using drinking_be.Interfaces;

namespace drinking_be.Models
{
    public class Banner : ISoftDelete
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!; // Link ảnh

        public string? Title { get; set; } // Tiêu đề (Alt text)

        public string? LinkUrl { get; set; } // Khi bấm vào banner thì nhảy đi đâu? (VD: /menu/tra-sua)

        public int SortOrder { get; set; } = 0; // Thứ tự hiển thị

        public string? Position { get; set; } // Vị trí: "Home-Top", "Sidebar", "Popup"

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}