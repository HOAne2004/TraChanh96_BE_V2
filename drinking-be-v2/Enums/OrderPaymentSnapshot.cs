namespace drinking_be.Domain.Orders
{
    public class OrderPaymentSnapshot
    {
        public decimal PaidAmount { get; init; }

        public bool HasAnyPayment => PaidAmount > 0;

        public bool IsFullyPaid(decimal grandTotal)
            => PaidAmount >= grandTotal;
    }
}
