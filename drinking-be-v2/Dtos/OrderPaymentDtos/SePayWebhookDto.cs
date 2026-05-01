using System.Text.Json.Serialization;

namespace drinking_be.Dtos.OrderPaymentDtos
{
    public class SePayWebhookDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("gateway")]
        public string? Gateway { get; set; }

        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        // 🟢 THÊM DẤU ? ĐỂ CHẤP NHẬN NULL
        [JsonPropertyName("subAccount")]
        public string? SubAccount { get; set; }

        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("transferType")]
        public string? TransferType { get; set; }

        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        // 🟢 THÊM DẤU ? ĐỂ CHẤP NHẬN NULL
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        // 🟢 Bổ sung thêm trường này vì trong Request của SePay có gửi về
        [JsonPropertyName("referenceCode")]
        public string? ReferenceCode { get; set; }
    }
}