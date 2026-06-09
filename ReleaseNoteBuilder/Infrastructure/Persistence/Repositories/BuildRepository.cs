using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Core.Interfaces;

namespace ReleaseNoteBuilder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Build entity
/// </summary>
public class BuildRepository : IBuildRepository
{
    private readonly ApplicationDbContext _context;

    public BuildRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Build>> GetAllAsync()
    {
        return await _context.Builds
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
    }

    public async Task<Build?> GetByIdAsync(int id)
    {
        return await _context.Builds
            .Include(b => b.WorkItems)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Build>> GetByProjectAsync(string project)
    {
        return await _context.Builds
            .Where(b => b.Project == project)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Build>> GetByStatusAsync(string status)
    {
        return await _context.Builds
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
    }

    public async Task<Build> AddAsync(Build build)
    {
        _context.Builds.Add(build);
        await _context.SaveChangesAsync();
        return build;
    }

    public async Task UpdateAsync(Build build)
    {
        _context.Builds.Update(build);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var build = await _context.Builds.FindAsync(id);
        if (build != null)
        {
            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedDataAsync()
    {
        if (await _context.Builds.AnyAsync())
            return;

        var builds = new List<Build>();
        var statuses = new[] { "Success", "Failed", "InProgress" };
        var projects = new[] { "WebApp", "API", "Mobile", "Dashboard" };
        var branches = new[] { "main", "develop", "feature/auth", "hotfix/bug-123" };

        for (int i = 1; i <= 20; i++)
        {
            builds.Add(new Build
            {
                Project = projects[i % projects.Length],
                Status = statuses[i % statuses.Length],
                Branch = branches[i % branches.Length],
                CreatedDate = DateTime.UtcNow.AddDays(-(20 - i)),
                CompletedDate = DateTime.UtcNow.AddDays(-(20 - i)).AddMinutes(30),
                SourceUrl = $"https://devops.azure.com/project/repo/build/{i}"
            });
        }

        _context.Builds.AddRange(builds);
        await _context.SaveChangesAsync();
    }
}