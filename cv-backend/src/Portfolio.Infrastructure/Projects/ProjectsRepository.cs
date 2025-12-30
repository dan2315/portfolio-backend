using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Projects.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

public class ProjectsRepository : IProjectsRepository
{
    private readonly AppDbContext _db;
    public ProjectsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync()
    {
        return await _db.Projects.Include(p => p.Reactions).ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null)
            return;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return _db.Projects.Include(p => p.Reactions).FirstOrDefault(p => p.Id == id);
    }

    public async Task UpdateAsync(Project project)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
    }

    public async Task<Project?> GetBySlug(string slug)
    {
        return _db.Projects.Include(p => p.Reactions).FirstOrDefault(p => p.Slug == slug);
    }

    public async Task AddReaction(ProjectReaction reaction)
    {
        _db.ProjectReactions.Add(reaction);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteReaction(ProjectReaction existingReaction)
    {
        _db.ProjectReactions.Remove(existingReaction);
        await _db.SaveChangesAsync();
    }
}