using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;

namespace ReleaseNoteBuilder.Application.Interfaces;

/// <summary>
/// Service interface for Build operations
/// </summary>
public interface IBuildService
{
    Task<List<BuildDto>> GetAllBuildsAsync();
    Task<BuildDto?> GetBuildByIdAsync(int id);
    Task<List<BuildDto>> GetBuildsByProjectAsync(string project);
    Task<List<BuildDto>> GetBuildsByStatusAsync(string status);
    Task<BuildDto> CreateBuildAsync(CreateBuildDto createDto);
    Task<BuildDto> UpdateBuildAsync(UpdateBuildDto updateDto);
    Task DeleteBuildAsync(int id);
}