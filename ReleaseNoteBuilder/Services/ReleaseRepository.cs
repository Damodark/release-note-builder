using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Data;
using ReleaseNoteBuilder.Models;



namespace ReleaseNoteBuilder.Services;

public class ReleaseRepository
{
    private readonly AppDbContext _db;

    public ReleaseRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task SeedData()
    {
        if (_db.Releases.Any())
            return;

        for (int i = 1; i <= 30; i++)
        {
            _db.Releases.Add(new ReleaseRecord
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

        await _db.SaveChangesAsync();
    }
// ✅ SAVE (Create new)
    public async Task Save(ReleaseRecord record)
    {
        _db.Releases.Add(record);
        await _db.SaveChangesAsync();
    }

    // ✅ GET ALL
    public async Task<List<ReleaseRecord>> GetAll()
    {
        return await _db.Releases
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    // ✅ UPDATE (Edit existing)
    public async Task Update(ReleaseRecord record)
    {
        _db.Releases.Update(record);
        await _db.SaveChangesAsync();
    }

    // ✅ DELETE
    public async Task Delete(int id)
    {
        var entity = await _db.Releases.FindAsync(id);

        if (entity != null)
        {
            _db.Releases.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }


}