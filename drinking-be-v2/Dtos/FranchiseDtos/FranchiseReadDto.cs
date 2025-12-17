namespace drinking_be.Dtos.FranchiseDtos
{
    public class FranchiseReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Address { get; set; }

        public string TargetArea { get; set; } = null!;
        public decimal? EstimatedBudget { get; set; }
        public string? ExperienceDescription { get; set; }

        // --- Quản trị ---
        public string Status { get; set; } = null!; // Enum String
        public string? AdminNote { get; set; }

        public int? ReviewerId { get; set; }
        public string? ReviewerName { get; set; } // Tên nhân viên phụ trách

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}