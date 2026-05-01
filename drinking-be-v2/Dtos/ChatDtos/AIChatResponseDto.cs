namespace drinking_be.Dtos.ChatDtos
{
    public class AIChatResponseDto
    {
        // Câu trả lời bằng văn bản của AI để hiển thị lên màn hình
        public string TextResponse { get; set; } = string.Empty;

        // Cờ báo hiệu cho Frontend biết AI vừa thực hiện hành động (như thêm vào giỏ hàng)
        // Nếu True, Frontend nên gọi lại API lấy chi tiết Giỏ hàng để cập nhật UI
        public bool IsCartUpdated { get; set; }
    }
}
