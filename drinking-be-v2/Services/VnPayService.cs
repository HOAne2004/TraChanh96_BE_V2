using drinking_be.Configs;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
namespace drinking_be.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _config;

        public VnPayService(IOptions<VnPayConfig> config)
        {
            _config = config.Value;
        }

        public string CreatePaymentUrl(Order order, HttpContext context)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnp_Returnurl = _config.ReturnUrl; // "https://localhost:5001/api/payments/vnpay-return"
            var vnp_Url = _config.VnPayUrl;
            var vnp_TmnCode = _config.TmnCode;
            var vnp_HashSecret = _config.HashSecret;

            var vnpParams = new SortedList<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((long)order.GrandTotal * 100).ToString() }, // VNPay nhân 100
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", Utils.Utils.GetIpAddress(context) }, // Hàm phụ trợ lấy IP
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"Thanh toan don hang {order.OrderCode}" },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", vnp_Returnurl },
            { "vnp_TxnRef", order.OrderCode } // Mã đơn hàng
        };

            // Build query string
            var signData = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

            // Tạo chữ ký (Signature)
            var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);

            // URL cuối cùng trả về FE
            return $"{vnp_Url}?{signData}&vnp_SecureHash={vnp_SecureHash}";
        }

        public bool ValidateSignature(IQueryCollection collections)
        {
            var vnpParams = new SortedList<string, string>();
            string vnp_SecureHash = collections["vnp_SecureHash"].ToString();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    vnpParams.Add(key, value.ToString());
                }
            }

            var signData = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            var checkSum = HmacSHA512(_config.HashSecret, signData);

            return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}
