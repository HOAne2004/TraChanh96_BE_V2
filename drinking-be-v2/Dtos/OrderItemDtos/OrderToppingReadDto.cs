// File: Dtos/OrderItemDtos/OrderToppingReadDto.cs

namespace drinking_be.Dtos.OrderItemDtos
{
    public class OrderToppingReadDto 
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
    }
}