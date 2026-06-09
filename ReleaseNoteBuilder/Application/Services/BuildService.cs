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
/// Service for Build business logic
/// </summary>
public class BuildService : IBuildService
{
    private readonly IBuildRepository _repository;

    public BuildService(IBuildRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BuildDto>> GetAllBuildsAsync()
    {
        var builds = await _repository.GetAllAsync();
        return builds.Select(MapToDto).ToList();
    }

    public async Task<BuildDto?> GetBuildByIdAsync(int id)
    {
        var build = await _repository.GetByIdAsync(id);
        return build != null ? MapToDto(build) : null;
    }

    public async Task<List<BuildDto>> GetBuildsByProjectAsync(string project)
    {
        if (string.IsNullOrWhiteSpace(project))
            throw new ArgumentException("Project name cannot be empty.", nameof(project));

        var builds = await _repository.GetByProjectAsync(project);
        return builds.Select(MapToDto).ToList();
    }

    public async Task<List<BuildDto>> GetBuildsByStatusAsync(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty.", nameof(status));

        var builds = await _repository.GetByStatusAsync(status);
        return builds.Select(MapToDto).ToList();
    }

    public async Task<BuildDto> CreateBuildAsync(CreateBuildDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Project))
            throw new ArgumentException("Project is required.", nameof(createDto.Project));

        var build = new Build
        {
            Project = createDto.Project,
            Status = createDto.Status ?? "InProgress",
            Branch = createDto.Branch ?? "develop",
            CreatedDate = DateTime.UtcNow,
            SourceUrl = createDto.SourceUrl
        };

        var created = await _repository.AddAsync(build);
        return MapToDto(created);
    }

    public async Task<BuildDto> UpdateBuildAsync(UpdateBuildDto updateDto)
    {
        var build = await _repository.GetByIdAsync(updateDto.Id);
        if (build == null)
            throw new InvalidOperationException($"Build with ID {updateDto.Id} not found.");

        build.Status = updateDto.Status ?? build.Status;
        build.Branch = updateDto.Branch ?? build.Branch;

        if (updateDto.CompletedDate.HasValue)
            build.CompletedDate = updateDto.CompletedDate;

        await _repository.UpdateAsync(build);
        return MapToDto(build);
    }

    public async Task DeleteBuildAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static BuildDto MapToDto(Build build)
    {
        return new BuildDto
        {
            Id = build.Id,
            Project = build.Project,
            Status = build.Status,
            Branch = build.Branch,
            CreatedDate = build.CreatedDate,
            CompletedDate = build.CompletedDate,
            SourceUrl = build.SourceUrl
        };
    }
}