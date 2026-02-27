namespace drinking_be.Dtos.EmailDtos
{
    public class ResetPasswordEmailModel
    {
        public string Username { get; set; } = string.Empty;
        public string ResetLink { get; set; } = string.Empty;
        public string CompanyName { get; set; } = "Trà chanh 96";
    }
}