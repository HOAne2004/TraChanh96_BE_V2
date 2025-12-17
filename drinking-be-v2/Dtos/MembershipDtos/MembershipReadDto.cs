// File: Dtos/MembershipDtos/MembershipReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.MembershipDtos
{
    public class MembershipReadDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!; // Cần Include User

        public string CardCode { get; set; } = null!;

        public byte LevelId { get; set; }
        public string LevelName { get; set; } = null!; // Cần Include Level

        public decimal? TotalSpent { get; set; }

        public DateOnly? LevelStartDate { get; set; }
        public DateOnly LevelEndDate { get; set; }
        public DateOnly? LastLevelSpentReset { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}