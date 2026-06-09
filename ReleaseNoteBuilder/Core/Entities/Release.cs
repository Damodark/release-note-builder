using System;
using System.Collections.Generic;

namespace ReleaseNoteBuilder.Core.Entities;

/// <summary>
/// Domain entity representing a Release
/// </summary>
public class Release
{
    public int Id { get; set; }
    public int BuildId { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ReleaseStatus Status { get; set; } = ReleaseStatus.Draft;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? DeployedAt { get; set; }
    public List<WorkItem> WorkItems { get; set; } = new();

    public bool IsProduction => Environment?.Equals("PROD", StringComparison.OrdinalIgnoreCase) ?? false;

    public void UpdateDetails(string project, int buildId, string environment, string branch, string notes)
    {
        Project = project;
        BuildId = buildId;
        Environment = environment;
        Branch = branch;
        Notes = notes;
    }
}
