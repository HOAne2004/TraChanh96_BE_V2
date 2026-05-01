using System;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ChatDtos
{
    public class ChatRequestDto
    {
        [Required(ErrorMessage = "Vui lòng chọn cửa hàng.")]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Thiếu SessionId.")]
        public Guid SessionId { get; set; }

        [Required(ErrorMessage = "Tin nhắn không được để trống.")]
        public string Message { get; set; } = string.Empty;
    }
}