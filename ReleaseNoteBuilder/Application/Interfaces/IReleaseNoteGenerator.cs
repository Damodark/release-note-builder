using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;

namespace ReleaseNoteBuilder.Application.Interfaces;

/// <summary>
/// Service for generating release notes
/// </summary>
public interface IReleaseNoteGenerator
{
    string GenerateReleaseNote(ReleaseDto release);
    string GenerateReleaseNotes(List<ReleaseDto> releases, string environment, int? buildId = null);
    Task<string> ExportToWordAsync(List<ReleaseDto> releases);
}
