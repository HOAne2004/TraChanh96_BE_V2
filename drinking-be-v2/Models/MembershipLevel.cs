using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using System.ComponentModel.DataAnnotations; // Thêm dòng này để dùng Range

public partial class MembershipLevel : ISoftDelete
{
    public int Id { get; set; }

    public string Name { get; set; } = null!; // Member, Silver, Gold, Diamond

    public short RankOrder { get; set; }

    public int MinCoinsRequired { get; set; }

    // --- CẤU HÌNH TÍCH ĐIỂM ---
    // 1 VNĐ = Bao nhiêu Xu?
    // VD: Rate = 0.01 (100đ = 1 xu). Đơn 100k = 1000 xu.
    public double PointEarningRate { get; set; }

    // --- CẤU HÌNH RESET (SOFT RESET) ---
    // Phần trăm số xu bị TRỪ khi reset chu kỳ.
    // VD: Diamond nhập 0.45 (Trừ 45%, giữ lại 55%)
    // VD: Silver nhập 0.65 (Trừ 65%, giữ lại 35%)
    [Range(0, 1)]
    public double ResetReductionPercent { get; set; }

    // --- CÁC TRƯỜNG CŨ ---
    public int? DurationDays { get; set; }
    public string? Benefits { get; set; }
    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public virtual ICollection<VoucherTemplate> VoucherTemplates { get; set; } = new List<VoucherTemplate>();
}