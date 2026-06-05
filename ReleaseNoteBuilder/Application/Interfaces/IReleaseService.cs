using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;

namespace ReleaseNoteBuilder.Application.Interfaces;

/// <summary>
/// Service interface for Release business logic
/// </summary>
public interface IReleaseService
{
    Task<List<ReleaseDto>> GetAllReleasesAsync();
    Task<ReleaseDto?> GetReleaseByIdAsync(int id);
    Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto createDto);
    Task UpdateReleaseAsync(UpdateReleaseDto updateDto);
    Task DeleteReleaseAsync(int id);
    Task<List<ReleaseDto>> GetReleasesByEnvironmentAsync(string environment);
    Task<List<ReleaseDto>> SearchReleasesAsync(string searchTerm, string? environmentFilter = null);
}
