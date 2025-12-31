namespace drinking_be.Models;

public class CommentLike
{
    public int UserId { get; set; }
    public int CommentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Comment Comment { get; set; } = null!;
}