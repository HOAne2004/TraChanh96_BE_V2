// File: Dtos/ShopTableDtos/ShopTableUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.ShopTableDtos
{
    public class ShopTableUpdateDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        public int? RoomId { get; set; }

        [Range(1, 20)]
        public byte? Capacity { get; set; }

        public bool? CanBeMerged { get; set; }

        // Cập nhật bàn mẹ (chỉ dùng khi thay đổi cấu hình gộp bàn)
        public int? MergedWithTableId { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}