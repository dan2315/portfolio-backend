using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Data;
using Portfolio.Application.Projects.DTOs;
using Portfolio.Application.Projects.Interfaces;

[ApiController]
[Route("/projects")]
public class ProjectsController : ControllerBase
{
    IProjectsService _projectsService;

    public ProjectsController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    [HttpGet("")]
    [RequireAnonSession]
    public async Task<ActionResult<IReadOnlyList<ProjectDTO>>> GetProjects()
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonSessionGuidKey]!;
        return Ok(await _projectsService.GetProjectsAsync(anonSession));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<DetailedProjectDTO>> GetProject(string slug)
    {
        return Ok(await _projectsService.GetProjectBySlugAsync(slug));
    }

    [HttpPost("{slug}/reaction")]
    [RequireAnonSession]
    public async Task<ActionResult<ProjectReactionsDTO>> AddReaction(string slug, [FromBody] string emoji)
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonSessionGuidKey]!;
        return await _projectsService.ToggleReactionAsync(slug, emoji, anonSession);
    }
}