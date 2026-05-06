using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.BannerDtos
{
    public class BannerUpdateDto
    {
        public int Id { get; set; }
        
        [Required]
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? LinkUrl { get; set; }
        public int SortOrder { get; set; }
        public string? Position { get; set; }
        public PublicStatusEnum Status { get; set; }
    }
}
