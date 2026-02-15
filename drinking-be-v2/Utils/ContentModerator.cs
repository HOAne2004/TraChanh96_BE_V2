using System.Text.RegularExpressions;

namespace drinking_be.Utils
{
    public static class ContentModerator
    {
        // Danh sách từ khóa cấm (Demo - Thực tế nên lưu trong DB hoặc Config file)
        private static readonly string[] _badWords = new[]
        {
            "lừa đảo", "lua dao", "ngu", "chó", "đm", "vkl", "đểu", "cút", "chết", "địt", "mẹ"
            // Thêm các từ khác...
        };

        public static (bool IsClean, string Reason) CheckContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content)) return (true, "");

            string lowerContent = content.ToLower();

            // 1. Check Spam Link (Chặn http, https, www, .com)
            // Regex đơn giản để bắt link
            if (Regex.IsMatch(lowerContent, @"(http|https|www|\.com|\.vn|\.net)"))
            {
                return (false, "Nội dung chứa liên kết quảng cáo không được phép.");
            }

            // 2. Check Từ tục tĩu
            foreach (var word in _badWords)
            {
                // Dùng word boundary \b để tránh bắt nhầm (VD: "chóng mặt" chứa "chó")
                // Tuy nhiên tiếng Việt không dấu khá phức tạp, ở đây check contains đơn giản trước
                if (lowerContent.Contains(word))
                {
                    return (false, "Nội dung chứa từ ngữ không phù hợp quy chuẩn cộng đồng.");
                }
            }

            return (true, "");
        }
    }
}