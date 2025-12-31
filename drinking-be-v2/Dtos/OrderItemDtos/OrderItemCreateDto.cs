using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderItemDtos
{
    public class OrderItemCreateDto
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100.")]
        public int Quantity { get; set; }

        public short? SizeId { get; set; }

        public SugarLevelEnum SugarLevel { get; set; } = SugarLevelEnum.S100;
        public IceLevelEnum IceLevel { get; set; } = IceLevelEnum.I100;

        [MaxLength(500)]
        public string? Note { get; set; }

        // Danh sách Topping (Mỗi topping cũng là 1 OrderItem nhưng là con)
        public List<OrderItemCreateDto> Toppings { get; set; } = new();
    }
}