using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReviewDtos
{
    // DTO này dùng cho API: PUT /api/reviews/{id}/admin-action
    public class ReviewAdminUpdateDto
    {
        public ReviewStatusEnum? Status { get; set; }

        [MaxLength(1000)]
        public string? AdminResponse { get; set; }
    }
}