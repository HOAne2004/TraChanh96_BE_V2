using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ChatDtos
{
    public class GenerateContentRequest
    {
        [Required(ErrorMessage = "Prompt không được để trống.")]
        [MaxLength(2000, ErrorMessage = "Prompt quá dài, vui lòng nhập dưới 2000 ký tự.")]
        public string Prompt { get; set; } = string.Empty;

        [Required(ErrorMessage = "ContentType không được để trống.")]
        public string ContentType { get; set; } = "product"; // Có thể là "product", "blog", "policy", v.v.
    }
}
