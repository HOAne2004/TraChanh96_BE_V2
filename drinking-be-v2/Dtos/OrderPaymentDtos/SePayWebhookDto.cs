using System.Text.Json.Serialization;

namespace drinking_be.Dtos.OrderPaymentDtos
{
    public class SePayWebhookDto
    {
        // ID giao dịch trên hệ thống SePay
        [JsonPropertyName("id")]
        public int Id { get; set; }

        // Tên ngân hàng (VD: Vietinbank, MBBank...)
        [JsonPropertyName("gateway")]
        public string Gateway { get; set; } = string.Empty;

        // Ngày giờ giao dịch
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; } = string.Empty;

        // Số tài khoản nhận tiền
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; } = string.Empty;

        // Tài khoản phụ (nếu có dùng tính năng định tuyến)
        [JsonPropertyName("subAccount")]
        public string SubAccount { get; set; } = string.Empty;

        // Số tiền giao dịch (Quan trọng)
        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        // Nội dung chuyển khoản (RẤT QUAN TRỌNG: Chứa mã OrderCode)
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        // Loại giao dịch: "in" (tiền vào) hoặc "out" (tiền ra)
        [JsonPropertyName("transferType")]
        public string TransferType { get; set; } = string.Empty;

        // Số dư tài khoản lũy kế
        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        // Mã giao dịch tham chiếu của ngân hàng (Mã GD)
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        // Mô tả thêm
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}