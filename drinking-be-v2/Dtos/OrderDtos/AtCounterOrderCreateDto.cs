namespace drinking_be.Dtos.OrderDtos
{
    public class AtCounterOrderCreateDto : BaseOrderCreateDto
    {
        public int? TableId { get; set; } // Null nếu mang về (Take Away)
    }
}
