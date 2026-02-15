using drinking_be.Dtos.Common;
using drinking_be.Enums;

namespace drinking_be.Dtos.ProductDtos
{
    public class ProductFilterDto : PagingRequest
    {
        public string? Keyword { get; set; }
        public ProductStatusEnum? Status { get; set; }
        public ProductTypeEnum? Type { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}