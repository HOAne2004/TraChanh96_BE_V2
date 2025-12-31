using drinking_be.Enums;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewReadDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; } // Nên thêm ảnh sản phẩm để hiển thị cho đẹp

        // ⭐️ Thêm thông tin đơn hàng để User biết họ review cho lần mua nào
        public long OrderId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? UserThumbnailUrl { get; set; }

        public string? Content { get; set; }
        public byte Rating { get; set; }
        public string? MediaUrl { get; set; }

        // Status trả về string (VD: "Pending", "Approved") thay vì Enum để FE dễ hiện
        public string Status { get; set; } = null!;
        public string? AdminResponse { get; set; }
        public bool IsEdited { get; set; } // Để hiện nhãn "(đã chỉnh sửa)"

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}