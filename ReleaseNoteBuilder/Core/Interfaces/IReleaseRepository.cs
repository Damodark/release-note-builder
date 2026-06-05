using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Core.Entities;

namespace ReleaseNoteBuilder.Core.Interfaces;

/// <summary>
/// Repository interface for Release entity
/// </summary>
public interface IReleaseRepository
{
    Task<List<Release>> GetAllAsync();
    Task<Release?> GetByIdAsync(int id);
    Task<Release> AddAsync(Release release);
    Task UpdateAsync(Release release);
    Task DeleteAsync(int id);
    Task SeedDataAsync();
}
