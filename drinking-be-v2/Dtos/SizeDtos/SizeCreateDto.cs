// File: Dtos/SizeDtos/SizeCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.SizeDtos
{
    public class SizeCreateDto
    {
        [Required(ErrorMessage = "Tên nhãn kích cỡ (Ví dụ: Lớn, Size L) không được để trống.")]
        [MaxLength(20)]
        public string Label { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá phụ thu không được để trống.")]
        [Range(0, 10000000, ErrorMessage = "Giá phụ thu không hợp lệ.")]
        public decimal? PriceModifier { get; set; } = 0m;

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}