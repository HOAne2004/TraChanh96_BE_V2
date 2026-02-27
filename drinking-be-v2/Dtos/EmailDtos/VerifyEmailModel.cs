namespace drinking_be.Dtos.EmailDtos
{
    public class VerifyEmailModel
    {
        public string Username { get; set; } = string.Empty;
        public string VerificationLink { get; set; } = string.Empty;
        public string Token { get; set; }
        public string CompanyName { get; set; } = "Trà Chanh 96";
    }

}