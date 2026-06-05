using System;

namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// Data Transfer Object for Release
/// </summary>
public class ReleaseDto
{
    public int Id { get; set; }
    public int BuildId { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
