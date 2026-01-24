using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

public class ProjectReactionsConfiguration : IEntityTypeConfiguration<ProjectReaction>
{
    public void Configure(EntityTypeBuilder<ProjectReaction> builder)
    {
        builder.ToTable("project_reactions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Emoji)
        .IsRequired()
        .HasMaxLength(50);

        builder.Property(r => r.AnonymousId)
        .IsRequired();

        builder.Property(r => r.CreatedAt)
        .HasDefaultValueSql("NOW()")
        .IsRequired();

        builder.HasOne(r => r.Project)
        .WithMany(p => p.Reactions)
        .HasForeignKey(r => r.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}