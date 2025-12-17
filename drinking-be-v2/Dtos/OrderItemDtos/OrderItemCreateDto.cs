// File: Dtos/OrderItemDtos/OrderItemCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.OrderItemDtos
{
    public class OrderItemCreateDto
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, 100)]
        public int Quantity { get; set; }

        public short? SizeId { get; set; }

        // ⭐ Sử dụng byte cho giá trị Enum
        public byte? SugarLevel { get; set; }
        public byte? IceLevel { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }

        // ⭐ Quan hệ đệ quy: Danh sách Topping (chính là OrderItemCreateDto)
        public List<OrderItemCreateDto> Toppings { get; set; } = new List<OrderItemCreateDto>();

        // ParentItemId không cần thiết ở DTO này; Service Layer sẽ tự gán sau khi tạo món chính.
    }
}