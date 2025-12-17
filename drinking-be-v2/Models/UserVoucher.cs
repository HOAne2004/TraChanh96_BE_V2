using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class UserVoucher : ISoftDelete
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public int VoucherTemplateId { get; set; }

    public string VoucherCode { get; set; } = null!;

    public DateTime? IssuedDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public UserVoucherStatusEnum Status { get; set; } = UserVoucherStatusEnum.Unused;
    public DateTime? UsedDate { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long? OrderIdUsed { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual VoucherTemplate VoucherTemplate { get; set; } = null!;
    public virtual Order? OrderUsed { get; set; }
}
