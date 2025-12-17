// Models/Address.cs
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;
using drinking_be.Enums;

namespace drinking_be.Models;

public partial class Address : ISoftDelete
{
    public long Id { get; set; } // Dùng long để đồng nhất với các ID chính

    // Liên kết với User (Nếu đây là địa chỉ của khách hàng). 
    // Cho phép null nếu Address này là địa chỉ của Store.
    public int? UserId { get; set; }

    // --- THÔNG TIN LIÊN HỆ ---
    // Cần thiết để shipper/cửa hàng biết giao cho ai và liên hệ khi cần
    public string? RecipientName { get; set; } = null!; // Họ tên người nhận
    public string? RecipientPhone { get; set; } = null!; // Số điện thoại người nhận

    // --- ĐỊA CHỈ CHI TIẾT VÀ TỌA ĐỘ ---
    public string FullAddress { get; set; } = null!; // Địa chỉ đầy đủ (Chuỗi hiển thị)
    public string AddressDetail { get; set; } = null!; // Chi tiết số nhà, tên đường

    public string Province { get; set; } = null!; // Tỉnh/Thành phố
    public string District { get; set; } = null!; // Quận/Huyện (Nên giữ lại)
    public string Commune { get; set; } = null!; // Xã/Phường

    public double Latitude { get; set; } // Vĩ độ (Cần cho tính phí ship thực tế)
    public double Longitude { get; set; } // Kinh độ (Cần cho tính phí ship thực tế)

    // --- QUẢN LÝ ---
    public bool? IsDefault { get; set; } // Địa chỉ mặc định của User

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    // --- NAVIGATION PROPERTIES ---
    public virtual User? User { get; set; } // Quan hệ: Một User có nhiều Address
    // Nếu Store cũng dùng bảng này, bạn cần thêm quan hệ ngược lại trong Store Entity.
    public virtual Store? Store { get; set; } 
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}