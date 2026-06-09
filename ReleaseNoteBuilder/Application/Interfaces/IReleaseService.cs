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
    Task<ReleaseDto> UpdateReleaseAsync(UpdateReleaseDto updateDto);
    Task DeleteReleaseAsync(int id);
    Task<List<ReleaseDto>> GetReleasesByEnvironmentAsync(string environment);
    Task<List<ReleaseDto>> SearchReleasesAsync(string searchTerm, string? environmentFilter = null);
    Task<ReleaseDto> DeployReleaseAsync(int id);
    Task<ReleaseDto> ApproveReleaseAsync(int id, string approvedBy = "");
    Task<ReleaseDto> RequestApprovalAsync(int id);
}
