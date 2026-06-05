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
/// Application service implementing release business logic
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
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(release);
        return MapToDto(created);
    }

    public async Task UpdateReleaseAsync(UpdateReleaseDto updateDto)
    {
        var release = await _repository.GetByIdAsync(updateDto.Id);
        if (release == null)
            throw new InvalidOperationException($"Release with ID {updateDto.Id} not found");

        release.UpdateDetails(
            updateDto.Project,
            updateDto.BuildId,
            updateDto.Environment,
            updateDto.Branch,
            updateDto.Notes
        );

        await _repository.UpdateAsync(release);
    }

    public async Task DeleteReleaseAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<List<ReleaseDto>> GetReleasesByEnvironmentAsync(string environment)
    {
        var releases = await _repository.GetAllAsync();
        return releases
            .Where(r => r.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase))
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<ReleaseDto>> SearchReleasesAsync(string searchTerm, string? environmentFilter = null)
    {
        var releases = await _repository.GetAllAsync();

        var filtered = releases.Where(r =>
            (string.IsNullOrEmpty(searchTerm) ||
             r.Project.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             r.Branch.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             r.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            &&
            (string.IsNullOrEmpty(environmentFilter) || 
             r.Environment.Equals(environmentFilter, StringComparison.OrdinalIgnoreCase))
        );

        return filtered.Select(MapToDto).ToList();
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
            CreatedAt = release.CreatedAt
        };
    }
}
