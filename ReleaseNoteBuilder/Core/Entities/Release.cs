using System;

namespace ReleaseNoteBuilder.Core.Entities;

/// <summary>
/// Domain entity representing a release
/// </summary>
public class Release
{
    public int Id { get; set; }
    public int BuildId { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsProduction() => Environment.Equals("PROD", StringComparison.OrdinalIgnoreCase);

    public void UpdateDetails(string project, int buildId, string environment, string branch, string notes)
    {
        Project = project;
        BuildId = buildId;
        Environment = environment;
        Branch = branch;
        Notes = notes;
    }
}
