using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Core.Entities;

namespace ReleaseNoteBuilder.Infrastructure.Persistence;

/// <summary>
/// Database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Release> Releases => Set<Release>();
    public DbSet<Build> Builds => Set<Build>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Release Configuration
        modelBuilder.Entity<Release>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Project).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Environment).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Branch).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => new { e.Environment, e.CreatedAt });
            entity.HasMany(r => r.WorkItems)
                .WithOne(w => w.Release)
                .HasForeignKey(w => w.ReleaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Build Configuration
        modelBuilder.Entity<Build>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Project).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Branch).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.Project, e.CreatedDate });
            entity.HasIndex(e => e.Status);
            entity.HasMany(b => b.WorkItems)
                .WithOne(w => w.Build)
                .HasForeignKey(w => w.BuildId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkItem Configuration
        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.AdoWorkItemId).HasMaxLength(100);
            entity.HasIndex(e => e.BuildId);
            entity.HasIndex(e => e.ReleaseId);
            entity.HasIndex(e => e.Type);
        });
    }
}
