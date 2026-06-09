using System;
using System.Collections.Generic;

namespace ReleaseNoteBuilder.Core.Entities;

/// <summary>
/// Domain entity representing a Build
/// </summary>
public class Build
{
    public int Id { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;  // Success, Failed, InProgress
    public string Branch { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? SourceUrl { get; set; }
    public List<WorkItem> WorkItems { get; set; } = new();

    public bool IsSuccessful => Status?.Equals("Success", StringComparison.OrdinalIgnoreCase) ?? false;
    public bool IsInProgress => Status?.Equals("InProgress", StringComparison.OrdinalIgnoreCase) ?? false;
}