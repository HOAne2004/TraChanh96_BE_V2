using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.CartDtos
{
    public class CartItemReadDto
    {
        public long Id { get; set; }
        public long CartId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? Note { get; set; }

        public short? SizeId { get; set; }
        public string? SizeLabel { get; set; }

        // ⭐ UPDATE: Bổ sung ID để Frontend dễ xử lý logic
        public short? SugarLevelId { get; set; }
        public string? SugarLabel { get; set; }

        public short? IceLevelId { get; set; }
        public string? IceLabel { get; set; }

        public ICollection<CartToppingReadDto> Toppings { get; set; } = new List<CartToppingReadDto>();
    }
}