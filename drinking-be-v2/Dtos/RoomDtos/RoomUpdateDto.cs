using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.RoomDtos
{
    public class RoomUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 1000)]
        public int? Capacity { get; set; }

        public bool? IsAirConditioned { get; set; }
        public bool? IsSmokingAllowed { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}