using drinking_be.Enums;

namespace drinking_be.Dtos.RoomDtos
{
    public class RoomReadDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!; // Cần Include Store

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Capacity { get; set; }

        public bool IsAirConditioned { get; set; }
        public bool IsSmokingAllowed { get; set; }

        public string Status { get; set; } = null!; // String Enum

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Có thể hiển thị số lượng bàn đang có trong phòng này
        public int TotalTables { get; set; }
    }
}