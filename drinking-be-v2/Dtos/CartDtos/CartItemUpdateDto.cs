using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.CartDtos
{
    public class CartItemUpdateDto
    {
        [Required]
        public long CartItemId { get; set; }

        [Range(0, 100)]
        public int Quantity { get; set; } // Nếu = 0 thì xóa
    }
}