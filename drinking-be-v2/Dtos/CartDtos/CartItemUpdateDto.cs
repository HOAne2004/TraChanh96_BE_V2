using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.CartDtos
{
    public class CartItemUpdateDto
    {
        [Required]
        public long CartItemId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được nhỏ hơn 0.")]
        public int Quantity { get; set; } 

        // Cho phép cập nhật lại các tùy chọn (Nullable để nếu không truyền thì giữ nguyên cũ)
        public short? SizeId { get; set; }
        public short? SugarLevelId { get; set; }
        public short? IceLevelId { get; set; }
        public string? Note { get; set; }

        // Danh sách topping mới (AI hoặc User truyền lên để thay thế danh sách cũ)
        public List<CartToppingCreateDto>? Toppings { get; set; }
    }
}