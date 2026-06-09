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
/// Service for Release business logic
/// </summary>
public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository _repository;

    public ReleaseService(IReleaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ReleaseDto>> GetAllReleasesAsync()
    {
        var releases = await _repository.GetAllAsync();
        return releases.Select(MapToDto).ToList();
    }

    public async Task<ReleaseDto?> GetReleaseByIdAsync(int id)
    {
        var release = await _repository.GetByIdAsync(id);
        return release != null ? MapToDto(release) : null;
    }

    public async Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto createDto)
    {
        var release = new Release
        {
            BuildId = createDto.BuildId,
            Project = createDto.Project,
            Environment = createDto.Environment,
            Branch = createDto.Branch,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow,
            Status = Core.Entities.ReleaseStatus.Draft,
            WorkItems = new List<WorkItem>()
        };

        var created = await _repository.AddAsync(release);
        return MapToDto(created);
    }

    public async Task<ReleaseDto> UpdateReleaseAsync(UpdateReleaseDto updateDto)
    {
        var release = await _repository.GetByIdAsync(updateDto.Id);
        if (release == null)
            throw new InvalidOperationException($"Release with ID {updateDto.Id} not found.");

        release.BuildId = updateDto.BuildId;
        release.Project = updateDto.Project;
        release.Environment = updateDto.Environment;
        release.Branch = updateDto.Branch;
        release.Notes = updateDto.Notes;

        await _repository.UpdateAsync(release);
        return MapToDto(release);
    }

    public async Task DeleteReleaseAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<List<ReleaseDto>> GetReleasesByEnvironmentAsync(string environment)
    {
        var releases = await _repository.GetAllAsync();
        var filtered = releases.Where(r => r.Environment == environment).ToList();
        return filtered.Select(MapToDto).ToList();
    }

    public async Task<List<ReleaseDto>> SearchReleasesAsync(string searchTerm, string? environmentFilter = null)
    {
        var releases = await _repository.GetAllAsync();
        var results = releases.Where(r =>
            r.Project.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            r.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            r.BuildId.ToString().Contains(searchTerm)
        ).ToList();

        if (!string.IsNullOrEmpty(environmentFilter))
        {
            results = results.Where(r => r.Environment == environmentFilter).ToList();
        }

        return results.Select(MapToDto).ToList();
    }

    public async Task<ReleaseDto> DeployReleaseAsync(int id)
    {
        var release = await _repository.GetByIdAsync(id);
        if (release == null)
            throw new InvalidOperationException($"Release with ID {id} not found.");

        if (release.Status != Core.Entities.ReleaseStatus.Approved)
            throw new InvalidOperationException("Only approved releases can be deployed.");

        release.Status = Core.Entities.ReleaseStatus.Deployed;
        release.DeployedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(release);
        return MapToDto(release);
    }

    public async Task<ReleaseDto> ApproveReleaseAsync(int id, string approvedBy = "")
    {
        var release = await _repository.GetByIdAsync(id);
        if (release == null)
            throw new InvalidOperationException($"Release with ID {id} not found.");

        release.Status = Core.Entities.ReleaseStatus.Approved;
        release.ApprovedAt = DateTime.UtcNow;
        release.ApprovedBy = approvedBy;

        await _repository.UpdateAsync(release);
        return MapToDto(release);
    }

    public async Task<ReleaseDto> RequestApprovalAsync(int id)
    {
        var release = await _repository.GetByIdAsync(id);
        if (release == null)
            throw new InvalidOperationException($"Release with ID {id} not found.");

        release.Status = Core.Entities.ReleaseStatus.PendingApproval;

        await _repository.UpdateAsync(release);
        return MapToDto(release);
    }

    private static ReleaseDto MapToDto(Release release)
    {
        return new ReleaseDto
        {
            Id = release.Id,
            BuildId = release.BuildId,
            Project = release.Project,
            Environment = release.Environment,
            Branch = release.Branch,
            Notes = release.Notes,
            CreatedAt = release.CreatedAt,
            Status = (Application.DTOs.ReleaseStatus)release.Status,
            ApprovedAt = release.ApprovedAt,
            ApprovedBy = release.ApprovedBy,
            DeployedAt = release.DeployedAt,
            WorkItems = release.WorkItems?.Select(wi => new WorkItemDto
            {
                Id = wi.Id,
                AdoWorkItemId = wi.AdoWorkItemId,
                Title = wi.Title,
                Type = wi.Type,
                Description = wi.Description,
                CreatedDate = wi.CreatedDate
            }).ToList() ?? new()
        };
    }
}
