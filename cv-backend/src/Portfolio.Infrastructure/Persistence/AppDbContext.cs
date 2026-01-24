using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Analytics;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence.Configurations;

namespace Portfolio.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();
    public DbSet<ActivitySession> ActivitySessions => Set<ActivitySession>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectReaction> ProjectReactions => Set<ProjectReaction>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ActivityEventsConfiguration());
        modelBuilder.ApplyConfiguration(new ActivitySessionsConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectsConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectReactionsConfiguration());
        modelBuilder.ApplyConfiguration(new MessagesConfiguration());
    }
}