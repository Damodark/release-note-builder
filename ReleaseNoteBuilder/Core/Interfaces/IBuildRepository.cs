using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Core.Entities;

namespace ReleaseNoteBuilder.Core.Interfaces;

/// <summary>
/// Repository interface for Build entity
/// </summary>
public interface IBuildRepository
{
    Task<List<Build>> GetAllAsync();
    Task<Build?> GetByIdAsync(int id);
    Task<List<Build>> GetByProjectAsync(string project);
    Task<List<Build>> GetByStatusAsync(string status);
    Task<Build> AddAsync(Build build);
    Task UpdateAsync(Build build);
    Task DeleteAsync(int id);
    Task SeedDataAsync();
}