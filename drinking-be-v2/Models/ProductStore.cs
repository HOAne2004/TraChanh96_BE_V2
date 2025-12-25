using drinking_be.Enums;
using drinking_be.Interfaces;
using System;

namespace drinking_be.Models;

public partial class ProductStore : ISoftDelete
{
    // --- COMPOSITE KEY ---
    public int ProductId { get; set; }
    public int StoreId { get; set; }

    // --- NGHIỆP VỤ BÁN ---
    // Số lượng đã bán tại cửa hàng này
    public int SoldCount { get; set; } = 0;
    public ProductStoreStatusEnum Status { get; set; }
        = ProductStoreStatusEnum.Available;

    // Giá override tại cửa hàng
    // Nếu null → dùng Product.BasePrice
    public decimal? PriceOverride { get; set; }

    public byte? SortOrder { get; set; }

    // --- AUDIT ---
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- NAVIGATION ---
    public virtual Product Product { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
}
