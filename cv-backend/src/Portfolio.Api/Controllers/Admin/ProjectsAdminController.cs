using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Data;
using Portfolio.Application.Projects.DTOs;
using Portfolio.Application.Projects.Interfaces;

[ApiController]
[Route("/admin/projects")]
public class ProjectsAdminController : ControllerBase
{
    private readonly IProjectsService _projectsService;

    public ProjectsAdminController(IProjectsService projectsService)
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDTO>> GetProjectById(string id)
    {
        var project = await _projectsService.GetProjectByIdAsync(Guid.Parse(id));
        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost("")]
    public async Task<ActionResult> CreateProject([FromBody] CreateProjectDTO projectDTO)
    {
        await _projectsService.CreateProject(projectDTO);
        return Ok();
    }

    [HttpPut("{id}")]
    void UpdateProject()
    {
        
    }

    [HttpPatch("{id}")]
    void TogglePublishProject()
    {
        
    }
}