namespace Portfolio.Application.Projects.DTOs;
public record CreateProjectDTO(
string Title,
string? ShortDescription,
string? Description,
int? PrideRating,
string[]? Technologies,
string? RepositoryURL,
bool IsPublished
);