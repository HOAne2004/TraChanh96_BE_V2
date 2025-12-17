using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class Room : ISoftDelete
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public string Name { get; set; } = null!; // Ví dụ: "Tầng 2", "Sân vườn", "Phòng lạnh VIP"

    public string? Description { get; set; }

    public int? Capacity { get; set; } // Sức chứa tối đa (số người) của phòng này

    // Các thuộc tính đặc điểm (Hỗ trợ lọc khi đặt bàn)
    public bool IsAirConditioned { get; set; } = true; // Có máy lạnh
    public bool IsSmokingAllowed { get; set; } = false; // Cho phép hút thuốc

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<ShopTable> ShopTables { get; set; } = new List<ShopTable>();
}