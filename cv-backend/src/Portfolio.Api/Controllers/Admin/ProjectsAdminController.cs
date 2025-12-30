using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Projects.DTOs;
using Portfolio.Application.Projects.Interfaces;

[ApiController]
[RequireAdminApiKey]
[Route("/admin/projects")]
public class ProjectsAdminController : ControllerBase
{
    private readonly IProjectsService _projectsService;

    public ProjectsAdminController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject([FromBody] CreateProjectDTO projectDTO)
    {
        await _projectsService.CreateProject(projectDTO);
        return Ok(new {message = "Project successfully created"});
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ProjectDTO>> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectDTO projectDTO)
    {
        var updatedProject = await _projectsService.UpdateProjectAsync(id, projectDTO);

        if (updatedProject == null)
            return NotFound();

        return Ok(updatedProject);
    }

    [HttpPatch("{id:guid}/publish")]
    public async Task<ActionResult> TogglePublishProject(Guid id)
    {
        var toggled = await _projectsService.TogglePublishAsync(id);

        if (!toggled)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        await _projectsService.DeleteProjectAsync(id);
        return Ok(new {message = "Project successfully deleted"});
    }
}
