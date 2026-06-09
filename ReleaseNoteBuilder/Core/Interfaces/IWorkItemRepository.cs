using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Core.Entities;

namespace ReleaseNoteBuilder.Core.Interfaces;

/// <summary>
/// Repository interface for WorkItem entity
/// </summary>
public interface IWorkItemRepository
{
    Task<List<WorkItem>> GetAllAsync();
    Task<WorkItem?> GetByIdAsync(int id);
    Task<List<WorkItem>> GetByBuildIdAsync(int buildId);
    Task<List<WorkItem>> GetByTypeAsync(string type);
    Task<List<WorkItem>> GetByReleaseIdAsync(int releaseId);
    Task<WorkItem> AddAsync(WorkItem workItem);
    Task UpdateAsync(WorkItem workItem);
    Task DeleteAsync(int id);
    Task SeedDataAsync();
}