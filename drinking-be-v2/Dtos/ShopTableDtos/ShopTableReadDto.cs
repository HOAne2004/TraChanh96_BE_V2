// File: Dtos/ShopTableDtos/ShopTableReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.ReservationDtos; // Nếu cần hiển thị Reservation

namespace drinking_be.Dtos.ShopTableDtos
{
    public class ShopTableReadDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int? RoomId { get; set; }
        public string StoreName { get; set; } = null!; // Cần Include Store
        public string? RoomName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public byte Capacity { get; set; }

        public bool? CanBeMerged { get; set; }

        // Thông tin Bàn mẹ (nếu bàn này là một phần của bàn lớn hơn)
        public int? MergedWithTableId { get; set; }
        public string? MergedWithTableName { get; set; } // Cần Include MergedWithTable

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Danh sách các bàn con đã gộp vào bàn này (nếu bàn này là bàn chính)
        public ICollection<ShopTableReadDto>? MergedTables { get; set; }

        // Navigation Properties (có thể được Include để xem đơn đặt trước)
        //public ICollection<ReservationReadDto> ActiveReservations { get; set; } = new List<ReservationReadDto>();
    }
}