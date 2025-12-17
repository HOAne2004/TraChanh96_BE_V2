// Utils/SlugGenerator.cs
using System.Text.RegularExpressions;

namespace drinking_be.Utils
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            // 1. Chuẩn hóa tiếng Việt (loại bỏ dấu)
            string normalized = phrase.Normalize(System.Text.NormalizationForm.FormD);
            var regex = new Regex(@"\p{Mn}", RegexOptions.None);
            string slug = regex.Replace(normalized, string.Empty).Normalize(System.Text.NormalizationForm.FormC);

            // 2. Chuyển thành chữ thường và thay thế khoảng trắng bằng gạch ngang
            slug = slug.ToLowerInvariant();

            // 3. Loại bỏ ký tự không phải chữ, số, hoặc dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // 4. Thay thế khoảng trắng và các dấu gạch ngang liên tiếp bằng một dấu gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-").Trim();
            slug = Regex.Replace(slug, @"-+", "-");

            // 5. Cắt bớt nếu quá dài (tùy chọn)
            if (slug.Length > 90)
            {
                slug = slug.Substring(0, 90);
            }

            return slug;
        }
    }
}