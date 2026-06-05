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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Release>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Project).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Environment).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Branch).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => new { e.Environment, e.CreatedAt });
        });
    }
}
