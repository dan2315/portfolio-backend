namespace Portfolio.Domain.Entities;

public class ProjectReaction
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Emoji { get; set; } = null!;
    public Guid AnonymousSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
