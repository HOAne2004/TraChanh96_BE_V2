// File: Dtos/SizeDtos/SizeUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.SizeDtos
{
    public class SizeUpdateDto
    {
        [MaxLength(20)]
        public string? Label { get; set; }

        [Range(0, 10000000)]
        public decimal? PriceModifier { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}