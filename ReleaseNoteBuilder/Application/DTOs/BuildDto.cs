using System;

namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// Data Transfer Object for Build
/// </summary>
public class BuildDto
{
    public int Id { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? SourceUrl { get; set; }
}