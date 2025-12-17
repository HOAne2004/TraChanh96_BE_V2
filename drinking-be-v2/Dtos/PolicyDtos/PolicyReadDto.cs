// File: Dtos/PolicyDtos/PolicyReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.PolicyDtos
{
    public class PolicyReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;

        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!; // Cần Include Brand

        public int? StoreId { get; set; }
        public string? StoreName { get; set; } // Cần Include Store

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}