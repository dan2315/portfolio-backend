using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class DailyActivitiesConfiguration : IEntityTypeConfiguration<DailyActivity>
{
    public void Configure(EntityTypeBuilder<DailyActivity> builder)
    {
        builder.ToTable("daily_activity");
    }
}