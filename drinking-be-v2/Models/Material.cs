using drinking_be.Enums;
using drinking_be.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public class Material : ISoftDelete
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // --- UNIT ---
    public MaterialUnit BaseUnit { get; set; }          // ml, gram
    public MaterialUnit? PurchaseUnit { get; set; }     // box, bottle

    public int ConversionRate { get; set; }             // 1 purchase = ? base

    // --- COST ---
    public decimal CostPerPurchaseUnit { get; set; }

    [NotMapped]
    public decimal CostPerBaseUnit =>
        ConversionRate > 0 ? CostPerPurchaseUnit / ConversionRate : 0;

    // --- STOCK CONTROL ---
    public int? MinStockAlert { get; set; }

    public bool IsActive { get; set; }

    // --- AUDIT ---
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- NAVIGATION ---
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();
}
