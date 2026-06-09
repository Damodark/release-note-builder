using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;

namespace ReleaseNoteBuilder.Application.Interfaces;

/// <summary>
/// Service interface for WorkItem operations
/// </summary>
public interface IWorkItemService
{
    Task<List<WorkItemDto>> GetAllWorkItemsAsync();
    Task<WorkItemDto?> GetWorkItemByIdAsync(int id);
    Task<List<WorkItemDto>> GetWorkItemsByBuildIdAsync(int buildId);
    Task<List<WorkItemDto>> GetWorkItemsByTypeAsync(string type);
    Task<List<WorkItemDto>> GetWorkItemsByReleaseIdAsync(int releaseId);
    Task<WorkItemDto> CreateWorkItemAsync(CreateWorkItemDto createDto);
    Task<WorkItemDto> UpdateWorkItemAsync(UpdateWorkItemDto updateDto);
    Task DeleteWorkItemAsync(int id);
}