using drinking_be.Enums;
using drinking_be.Interfaces;

public partial class Staff : ISoftDelete
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public int UserId { get; set; }
    public int? StoreId { get; set; }

    public string FullName { get; set; } = null!;
    public string? CitizenId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }

    public StaffPositionEnum Position { get; set; } = StaffPositionEnum.Server;

    public DateTime HireDate { get; set; }

    public SalaryTypeEnum SalaryType { get; set; } = SalaryTypeEnum.PartTime;

    public decimal? BaseSalary { get; set; }
    public decimal? HourlySalary { get; set; }
    public decimal? OvertimeHourlySalary { get; set; }

    public double? MinWorkHoursPerMonth { get; set; }
    public double? MaxWorkHoursPerMonth { get; set; }
    public double? MaxOvertimeHoursPerMonth { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Store? Store { get; set; }
}
