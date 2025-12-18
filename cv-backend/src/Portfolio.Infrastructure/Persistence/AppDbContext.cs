
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options){}

    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityEvent>(entity =>
        {
            entity.ToTable("activity_events");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.EventType).IsRequired();
            entity.Property(x => x.Route).IsRequired();
            entity.Property(x => x.Method).IsRequired();
            entity.Property(x => x.Timestamp).IsRequired();
        });
    }
}
