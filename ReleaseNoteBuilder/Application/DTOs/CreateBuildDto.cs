namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for creating a new Build
/// </summary>
public class CreateBuildDto
{
    public string Project { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
}