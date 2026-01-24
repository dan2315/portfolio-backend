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
    [RequireAnonIdentity]
    public async Task<ActionResult<IReadOnlyList<ProjectDTO>>> GetProjects()
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonIdCookieName]!;
        return Ok(await _projectsService.GetProjectsAsync(anonSession));
    }

    [HttpGet("{slug}")]
    [RequireAnonIdentity]
    public async Task<ActionResult<ProjectDTO>> GetProject(string slug)
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonIdCookieName]!;
        var project = await _projectsService.GetProjectBySlugAsync(slug, anonSession);
        if (project == null)
        return NotFound();

        return Ok(project);
    }

    [HttpGet("{id}")]
    [RequireAnonIdentity]
    public async Task<ActionResult<ProjectDTO>> GetProjectById(string id)
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonIdCookieName]!;
        var project = await _projectsService.GetProjectByIdAsync(Guid.Parse(id), anonSession);
        if (project == null)
        return NotFound();

        return Ok(project);
    }

    [HttpPost("{slug}/reaction")]
    [RequireAnonIdentity]
    public async Task<ActionResult<ProjectReactionsDTO>> AddReaction(string slug, [FromBody] string emoji)
    {
        var anonSession = (Guid) HttpContext.Items[Keys.AnonIdCookieName]!;
        return await _projectsService.ToggleReactionAsync(slug, emoji, anonSession);
    }
}