namespace drinking_be.Dtos.OrderPaymentDtos
{
    public class CreateChargeRequestDto
    {
        public long OrderId { get; set; }
        public int PaymentMethodId { get; set; }
    }
    public class PaymentCallbackDto
    {
        public string TransactionCode { get; set; } = null!;
        public string? Reason { get; set; }
    }
    public class RefundRequestDto
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
    }
}
