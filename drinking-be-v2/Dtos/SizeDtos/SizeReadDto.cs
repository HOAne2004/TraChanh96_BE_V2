// File: Dtos/SizeDtos/SizeReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.SizeDtos
{
    public class SizeReadDto
    {
        public short Id { get; set; }
        public string Label { get; set; } = null!;
        public decimal? PriceModifier { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}