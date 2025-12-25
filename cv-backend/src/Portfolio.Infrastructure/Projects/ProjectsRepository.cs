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
        _db.SaveChanges();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Project?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Project project)
    {
        throw new NotImplementedException();
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