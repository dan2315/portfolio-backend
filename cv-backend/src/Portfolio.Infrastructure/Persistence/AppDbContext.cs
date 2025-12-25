using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence.Configurations;

namespace Portfolio.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectReaction> ProjectReactions => Set<ProjectReaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ActivityEventsConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectsConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectReactionsConfiguration());
    }
}