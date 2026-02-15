using drinking_be.Dtos.Common; // Chứa PagingRequest
using drinking_be.Enums;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewFilterDto : PagingRequest
    {
        public int? ProductId { get; set; }
        public Guid? UserPublicId { get; set; }
        public int? StoreId { get; set; }
        public ReviewStatusEnum? Status { get; set; }
        public byte? Rating { get; set; } 
        public string? Keyword { get; set; } 
        public bool? HasReply { get; set; } 
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}