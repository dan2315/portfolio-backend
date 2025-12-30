namespace Portfolio.Application.Projects.DTOs
{
    public class UpdateProjectDTO
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string[]? Technologies { get; set; }
        public string? Description { get; set; }
        public int? PrideRating { get; set; }
        public string? RepositoryURL { get; set; }
        public bool? IsPublished { get; set; }
    }
}