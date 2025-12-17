using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.RoomDtos
{
    public class RoomCreateDto
    {
        [Required(ErrorMessage = "Mã cửa hàng không được để trống.")]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Tên khu vực/phòng không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Sức chứa không hợp lệ.")]
        public int Capacity { get; set; } = 0;

        public bool IsAirConditioned { get; set; } = true;
        public bool IsSmokingAllowed { get; set; } = false;

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}