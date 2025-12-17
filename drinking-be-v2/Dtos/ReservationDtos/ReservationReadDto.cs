// File: Dtos/ReservationDtos/ReservationReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.ReservationDtos
{
    public class ReservationReadDto
    {
        public long Id { get; set; }
        public string ReservationCode { get; set; } = null!;

        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!; // Cần Include Store

        public int? UserId { get; set; }
        public string? UserName { get; set; } // Cần Include User

        public DateTime ReservationDatetime { get; set; }
        public byte NumberOfGuests { get; set; }

        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? Note { get; set; }

        // --- Bàn được gán ---
        public int? AssignedTableId { get; set; }
        public string? AssignedTableName { get; set; } // Cần Include AssignedTable

        // --- Đặt cọc ---
        public decimal DepositAmount { get; set; }
        public bool IsDepositPaid { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}