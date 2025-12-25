using drinking_be.Models;
using System.ComponentModel.DataAnnotations;

public partial class Inventory
{
    public long Id { get; set; }

    public Guid PublicId { get; set; }

    // --- FK ---
    public int MaterialId { get; set; }
    public int? StoreId { get; set; } // NULL = kho tổng

    // --- QUANTITY (Base Unit) ---
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime UpdatedAt { get; set; }

    // --- CONCURRENCY ---
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    // --- NAVIGATION ---
    public virtual Material Material { get; set; } = null!;
    public virtual Store? Store { get; set; }
}
