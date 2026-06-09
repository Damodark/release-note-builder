using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;
using ReleaseNoteBuilder.Application.Interfaces;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Core.Interfaces;

namespace ReleaseNoteBuilder.Application.Services;

/// <summary>
/// Service for WorkItem business logic
/// </summary>
public class WorkItemService : IWorkItemService
{
    private readonly IWorkItemRepository _repository;

    public WorkItemService(IWorkItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WorkItemDto>> GetAllWorkItemsAsync()
    {
        var workItems = await _repository.GetAllAsync();
        return workItems.Select(MapToDto).ToList();
    }

    public async Task<WorkItemDto?> GetWorkItemByIdAsync(int id)
    {
        var workItem = await _repository.GetByIdAsync(id);
        return workItem != null ? MapToDto(workItem) : null;
    }

    public async Task<List<WorkItemDto>> GetWorkItemsByBuildIdAsync(int buildId)
    {
        var workItems = await _repository.GetByBuildIdAsync(buildId);
        return workItems.Select(MapToDto).ToList();
    }

    public async Task<List<WorkItemDto>> GetWorkItemsByTypeAsync(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty.", nameof(type));

        var workItems = await _repository.GetByTypeAsync(type);
        return workItems.Select(MapToDto).ToList();
    }

    public async Task<List<WorkItemDto>> GetWorkItemsByReleaseIdAsync(int releaseId)
    {
        var workItems = await _repository.GetByReleaseIdAsync(releaseId);
        return workItems.Select(MapToDto).ToList();
    }

    public async Task<WorkItemDto> CreateWorkItemAsync(CreateWorkItemDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Title))
            throw new ArgumentException("Title is required.", nameof(createDto.Title));

        var workItem = new WorkItem
        {
            AdoWorkItemId = createDto.AdoWorkItemId ?? "",
            Title = createDto.Title,
            Type = createDto.Type ?? "Task",
            Description = createDto.Description ?? "",
            CreatedDate = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(workItem);
        return MapToDto(created);
    }

    public async Task<WorkItemDto> UpdateWorkItemAsync(UpdateWorkItemDto updateDto)
    {
        var workItem = await _repository.GetByIdAsync(updateDto.Id);
        if (workItem == null)
            throw new InvalidOperationException($"WorkItem with ID {updateDto.Id} not found.");

        workItem.Title = updateDto.Title ?? workItem.Title;
        workItem.Type = updateDto.Type ?? workItem.Type;
        workItem.Description = updateDto.Description ?? workItem.Description;

        await _repository.UpdateAsync(workItem);
        return MapToDto(workItem);
    }

    public async Task DeleteWorkItemAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static WorkItemDto MapToDto(WorkItem workItem)
    {
        return new WorkItemDto
        {
            Id = workItem.Id,
            AdoWorkItemId = workItem.AdoWorkItemId,
            Title = workItem.Title,
            Type = workItem.Type,
            Description = workItem.Description,
            CreatedDate = workItem.CreatedDate
        };
    }
}