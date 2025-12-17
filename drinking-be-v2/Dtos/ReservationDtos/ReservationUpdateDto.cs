using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReservationDtos
{
    public class ReservationUpdateDto
    {
        public ReservationStatusEnum? Status { get; set; }

        public int? AssignedTableId { get; set; } // Admin gán bàn

        public bool? IsDepositPaid { get; set; } // Xác nhận đã đóng tiền cọc

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}