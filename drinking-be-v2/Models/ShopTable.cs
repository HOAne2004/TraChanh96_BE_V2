using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class ShopTable : ISoftDelete
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int? RoomId { get; set; }

    public string Name { get; set; } = null!;

    public byte Capacity { get; set; }

    public bool? CanBeMerged { get; set; }

    public int? MergedWithTableId { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ShopTable> InverseMergedWithTable { get; set; } = new List<ShopTable>();

    public virtual ShopTable? MergedWithTable { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Store Store { get; set; } = null!;

    public virtual Room? Room { get; set; }
}
