using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Application.Analytics;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ActivityEventsConfiguration : IEntityTypeConfiguration<ActivityEvent>
{
    public void Configure(EntityTypeBuilder<ActivityEvent> builder)
    {
        builder.ToTable("activity_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType).IsRequired();
        builder.Property(x => x.Route).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();
        builder.Property(x => x.Processed).HasDefaultValue(true);
    }
}