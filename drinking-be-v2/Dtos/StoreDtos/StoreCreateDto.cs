// File: Dtos/StoreDtos/StoreCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.StoreDtos
{
    public class StoreCreateDto
    {
        [Required(ErrorMessage = "Tên cửa hàng không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã Brand không được để trống.")]
        public int BrandId { get; set; }

        // ⭐ AddressId là bắt buộc, phải trỏ tới một bản ghi Address đã tồn tại
        [Required(ErrorMessage = "Mã địa chỉ không được để trống.")]
        public long AddressId { get; set; }

        public string? ImageUrl { get; set; }

        // Giờ hoạt động
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public DateTime? OpenDate { get; set; } // Ngày khai trương

        // Cấu hình phí ship
        public decimal? ShippingFeeFixed { get; set; } = 0m;
        public decimal? ShippingFeePerKm { get; set; } = 0m;

        public StoreStatusEnum Status { get; set; } = StoreStatusEnum.ComingSoon;

        public byte? SortOrder { get; set; }
        public bool? MapVerified { get; set; } = false;

        // Slug sẽ được Service Layer tính toán
    }
}