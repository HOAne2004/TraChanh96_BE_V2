// File: Dtos/ReservationDtos/ReservationCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReservationDtos
{
    public class ReservationCreateDto
    {
        // UserId có thể nullable (Khách vãng lai)
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Mã Store không được để trống.")]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Thời gian đặt bàn không được để trống.")]
        public DateTime ReservationDatetime { get; set; }

        [Required(ErrorMessage = "Số lượng khách không được để trống.")]
        [Range(1, 20, ErrorMessage = "Số lượng khách phải từ 1 đến 20.")]
        public byte NumberOfGuests { get; set; }

        // --- Thông tin liên hệ ---
        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string CustomerPhone { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        // --- Thông tin Đặt cọc ---
        [Required(ErrorMessage = "Số tiền đặt cọc không được để trống.")]
        [Range(0, 10000000, ErrorMessage = "Số tiền đặt cọc không hợp lệ.")]
        public decimal DepositAmount { get; set; } = 0m;

        public bool IsDepositPaid { get; set; } = false;

        // Trạng thái (Mặc định là Pending/Chờ xử lý, không cần đưa vào DTO)
        // AssignedTableId cũng sẽ được Service Layer gán sau khi xử lý
    }
}