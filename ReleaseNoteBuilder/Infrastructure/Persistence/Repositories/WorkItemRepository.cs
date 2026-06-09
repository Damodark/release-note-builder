using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Core.Interfaces;

namespace ReleaseNoteBuilder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WorkItem entity
/// </summary>
public class WorkItemRepository : IWorkItemRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkItem>> GetAllAsync()
    {
        return await _context.WorkItems
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync();
    }

    public async Task<WorkItem?> GetByIdAsync(int id)
    {
        return await _context.WorkItems
            .Include(w => w.Build)
            .Include(w => w.Release)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<List<WorkItem>> GetByBuildIdAsync(int buildId)
    {
        return await _context.WorkItems
            .Where(w => w.BuildId == buildId)
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkItem>> GetByTypeAsync(string type)
    {
        return await _context.WorkItems
            .Where(w => w.Type == type)
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<WorkItem>> GetByReleaseIdAsync(int releaseId)
    {
        return await _context.WorkItems
            .Where(w => w.ReleaseId == releaseId)
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync();
    }

    public async Task<WorkItem> AddAsync(WorkItem workItem)
    {
        _context.WorkItems.Add(workItem);
        await _context.SaveChangesAsync();
        return workItem;
    }

    public async Task UpdateAsync(WorkItem workItem)
    {
        _context.WorkItems.Update(workItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var workItem = await _context.WorkItems.FindAsync(id);
        if (workItem != null)
        {
            _context.WorkItems.Remove(workItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedDataAsync()
    {
        if (await _context.WorkItems.AnyAsync())
            return;

        var workItems = new List<WorkItem>();
        var types = new[] { "Bug", "Feature", "User Story", "Task" };
        var titles = new[]
        {
            "Add authentication", "Fix login bug", "Add profile page", "Performance optimization",
            "Update UI theme", "Fix data validation", "Add export feature", "Improve search"
        };

        for (int buildId = 1; buildId <= 10; buildId++)
        {
            for (int i = 1; i <= 4; i++)
            {
                workItems.Add(new WorkItem
                {
                    AdoWorkItemId = $"WI-{buildId}{i:D2}",
                    Title = titles[(buildId * i - 1) % titles.Length],
                    Type = types[(buildId + i) % types.Length],
                    Description = $"Work item for build {buildId}",
                    CreatedDate = DateTime.UtcNow.AddDays(-(20 - buildId)),
                    BuildId = buildId
                });
            }
        }

        _context.WorkItems.AddRange(workItems);
        await _context.SaveChangesAsync();
    }
}