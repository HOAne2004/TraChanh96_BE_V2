namespace drinking_be.Configs
{
    public class VnPayConfig
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string VnPayUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
