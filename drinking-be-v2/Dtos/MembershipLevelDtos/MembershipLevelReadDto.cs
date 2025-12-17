// File: Dtos/MembershipLevelDtos/MembershipLevelReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.MembershipLevelDtos
{
    public class MembershipLevelReadDto
    {
        public byte Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal MinSpendRequired { get; set; }
        public short DurationDays { get; set; }
        public string? Benefits { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ⭐ Có thể bổ sung số lượng thành viên/voucher liên quan (Nếu cần)
        public int MemberCount { get; set; }
        public int VoucherTemplateCount { get; set; }
    }
}