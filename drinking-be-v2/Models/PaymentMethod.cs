using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class PaymentMethod : ISoftDelete
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public PaymentTypeEnum PaymentType { get; set; } = PaymentTypeEnum.COD;

    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankAccountName { get; set; }
    public string? Instructions { get; set; }
    public string? QRTplUrl { get; set; } 
    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public byte? SortOrder { get; set; }

    public decimal? ProcessingFee { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
