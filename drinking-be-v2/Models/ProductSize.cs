// Models/ProductSize.cs
namespace drinking_be.Models
{
    public class ProductSize
    {
        // Khóa kép
        public int ProductId { get; set; }
        public short SizeId { get; set; }

        // Navigation Properties
        public Product Product { get; set; } = null!;
        public Size Size { get; set; } = null!;


    }
}