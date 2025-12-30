namespace Portfolio.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? ShortDescription { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public int? PrideRating { get; set; }
    public string[]? Technologies { get; set; }
    public string? RepositoryUrl { get; set; }
    public bool IsPublished { get; set; }
    public ICollection<ProjectReaction> Reactions { get; set; } = new List<ProjectReaction>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
