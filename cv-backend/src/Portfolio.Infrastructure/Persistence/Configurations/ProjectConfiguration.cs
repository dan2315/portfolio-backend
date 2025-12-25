using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations
{
    public class ProjectsConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("projects");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.ShortDescription)
                   .HasMaxLength(500);

            builder.Property(p => p.PrideRating)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}