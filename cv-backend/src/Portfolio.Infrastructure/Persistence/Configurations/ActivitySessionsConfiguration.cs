using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ActivitySessionsConfiguration : IEntityTypeConfiguration<ActivitySession>
{
    public void Configure(EntityTypeBuilder<ActivitySession> builder)
    {
        builder.ToTable("activity_sessions");
    }
}