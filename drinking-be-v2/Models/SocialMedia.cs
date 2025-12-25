using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using drinking_be.Enums;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class SocialMedia : ISoftDelete
{
    public int Id { get; set; }
    public int? BrandId { get; set; }
    public int? StoreId { get; set; }
    public SocialPlatformEnum Platform { get; set; }
    public string Url { get; set; } = null!;
    public string? IconUrl { get; set; }
    public byte? SortOrder { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    public virtual Brand? Brand { get; set; } = null!;
    public virtual Store? Store { get; set; } = null!;
}
