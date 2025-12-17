using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.MaterialDtos
{
    public class MaterialUpdateDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        [MaxLength(50)]
        public string? BaseUnit { get; set; }

        [MaxLength(50)]
        public string? PurchaseUnit { get; set; }

        [Range(1, 10000)]
        public int? ConversionRate { get; set; }

        [Range(0, 1000000000)]
        public decimal? CostPerPurchaseUnit { get; set; }

        public int? MinStockAlert { get; set; }
        public bool? IsActive { get; set; }
    }
}