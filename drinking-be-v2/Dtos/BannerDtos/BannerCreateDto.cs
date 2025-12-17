using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.BannerDtos
{
    public class BannerCreateDto
    {
        [Required]
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? LinkUrl { get; set; }
        public int SortOrder { get; set; } = 0;
        public string? Position { get; set; } = "Home-Top";
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}