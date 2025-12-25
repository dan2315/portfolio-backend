namespace Portfolio.Domain.Entities;

public class Experience
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string? Location { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string[] Technologies { get; set; }
    public string Description { get; set; } = null!;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
