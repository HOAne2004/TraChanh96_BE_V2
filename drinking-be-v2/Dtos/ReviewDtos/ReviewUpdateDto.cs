using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewUpdateDto
    {
        // Admin duyệt hoặc từ chối
        public ReviewStatusEnum? Status { get; set; }

        // Admin trả lời (Cảm ơn hoặc xin lỗi khách)
        [MaxLength(1000)]
        public string? AdminResponse { get; set; }
    }
}