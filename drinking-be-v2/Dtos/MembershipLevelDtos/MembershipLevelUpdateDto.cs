// File: Dtos/MembershipLevelDtos/MembershipLevelUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.MembershipLevelDtos
{
    public class MembershipLevelUpdateDto
    {
        [MaxLength(35)]
        public string? Name { get; set; }

        [Range(0, 1000000000)]
        public decimal? MinSpendRequired { get; set; }

        [Range(1, 1000)]
        public short? DurationDays { get; set; }

        public string? Benefits { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}