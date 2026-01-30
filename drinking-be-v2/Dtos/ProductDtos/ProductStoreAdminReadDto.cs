using drinking_be.Enums;

namespace drinking_be.Dtos.ProductDtos
{
    public class ProductStoreAdminReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public ProductStoreStatusEnum Status { get; set; }
    }
}
