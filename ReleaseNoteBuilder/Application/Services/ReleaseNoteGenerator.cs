using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseNoteBuilder.Application.DTOs;
using ReleaseNoteBuilder.Application.Interfaces;

namespace ReleaseNoteBuilder.Application.Services;

/// <summary>
/// Service for generating release notes in various formats
/// </summary>
public class ReleaseNoteGenerator : IReleaseNoteGenerator
{
    public string GenerateReleaseNote(ReleaseDto release)
    {
        return $"Release {release.Environment} Build {release.BuildId}\n" +
               $"Project: {release.Project}\n" +
               $"Branch: {release.Branch}\n" +
               $"Date: {release.CreatedAt:yyyy-MM-dd HH:mm}\n\n" +
               $"Notes:\n{release.Notes}";
    }

    public string GenerateReleaseNotes(List<ReleaseDto> releases, string environment, int? buildId = null)
    {
        var filtered = releases
            .Where(r => r.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase))
            .Where(r => !buildId.HasValue || r.BuildId == buildId.Value);

        return string.Join("\n\n---\n\n", filtered.Select(GenerateReleaseNote));
    }

    public async Task<string> ExportToWordAsync(List<ReleaseDto> releases)
    {
        await Task.CompletedTask; // Placeholder for async operation

        var content = string.Join("\n\n", releases.Select(r =>
            $"Project: {r.Project}\n" +
            $"Build: {r.BuildId}\n" +
            $"Env: {r.Environment}\n" +
            $"Branch: {r.Branch}\n" +
            $"Date: {r.CreatedAt:yyyy-MM-dd HH:mm}\n\n" +
            $"Notes:\n{r.Notes}"
        ));

        return content;
    }
}
