using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Core.Interfaces;

namespace ReleaseNoteBuilder.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Release entity
/// </summary>
public class ReleaseRepository : IReleaseRepository
{
    private readonly ApplicationDbContext _context;

    public ReleaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Release>> GetAllAsync()
    {
        return await _context.Releases
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Release?> GetByIdAsync(int id)
    {
        return await _context.Releases.FindAsync(id);
    }

    public async Task<Release> AddAsync(Release release)
    {
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();
        return release;
    }

    public async Task UpdateAsync(Release release)
    {
        _context.Releases.Update(release);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var release = await _context.Releases.FindAsync(id);
        if (release != null)
        {
            _context.Releases.Remove(release);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedDataAsync()
    {
        if (await _context.Releases.AnyAsync())
            return;

        var releases = new List<Release>();

        for (int i = 1; i <= 30; i++)
        {
            releases.Add(new Release
            {
                BuildId = i,
                Project = "ACG",
                Environment = (i % 4) switch
                {
                    0 => "PROD",
                    1 => "UAT",
                    2 => "TEST",
                    _ => "DEV"
                },
                Branch = "develop",
                Notes = $"Release notes for build {i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        _context.Releases.AddRange(releases);
        await _context.SaveChangesAsync();
    }
}
