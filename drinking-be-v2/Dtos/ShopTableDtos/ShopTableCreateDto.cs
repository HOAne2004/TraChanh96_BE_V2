// File: Dtos/ShopTableDtos/ShopTableCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.ShopTableDtos
{
    public class ShopTableCreateDto
    {
        [Required(ErrorMessage = "Mã Store không được để trống.")]
        public int StoreId { get; set; }
        public int? RoomId { get; set; }

        [Required(ErrorMessage = "Tên bàn không được để trống.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sức chứa không được để trống.")]
        [Range(1, 20, ErrorMessage = "Sức chứa phải từ 1 đến 20.")]
        public byte Capacity { get; set; }

        public bool? CanBeMerged { get; set; } = true;

        // Nếu bàn này được tạo ra để là một phần của bàn gộp, ta gán MergedWithTableId
        public int? MergedWithTableId { get; set; }

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}