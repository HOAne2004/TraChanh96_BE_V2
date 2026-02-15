// Models/OrderItem.cs
using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

[Table("OrderItems")]
public partial class OrderItem
{
    [Key]
    public long Id { get; set; }
    public long OrderId { get; set; }
    public int ProductId { get; set; }

    // --- SNAPSHOT (Lưu cứng thông tin lúc mua) ---
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = null!;

    [MaxLength(500)]
    public string? ProductImage { get; set; }

    // [NEW] Lưu cứng tên Size (VD: "Size L", "Size M")
    [MaxLength(50)]
    public string? SizeName { get; set; }
    public decimal? SizePrice { get; set; }

    // ----------------------------------------------

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; } // Giá gốc 1 đơn vị

    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalPrice { get; set; } // 🔑 TỔNG = Unit * Quantity

    [MaxLength(500)]
    public string? Note { get; set; }

    // --- TOPPING (Đệ quy) ---
    public long? ParentItemId { get; set; }

    public short? SizeId { get; set; } // Vẫn giữ ID để tham chiếu nếu cần thống kê

    public SugarLevelEnum SugarLevel { get; set; } = SugarLevelEnum.S100;
    public IceLevelEnum IceLevel { get; set; } = IceLevelEnum.I100;

    // --- NAVIGATION ---
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey(nameof(SizeId))]
    public virtual Size? Size { get; set; }

    [ForeignKey(nameof(ParentItemId))]
    public virtual OrderItem? ParentItem { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }

    public virtual ICollection<OrderItem> InverseParentItem { get; set; } = new List<OrderItem>();
}