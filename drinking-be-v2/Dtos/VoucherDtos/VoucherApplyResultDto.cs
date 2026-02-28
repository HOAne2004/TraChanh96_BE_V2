namespace drinking_be.Dtos.VoucherDtos
{
    public class VoucherApplyResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string VoucherCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; } 
        public long? UserVoucherId { get; set; }
    }
}