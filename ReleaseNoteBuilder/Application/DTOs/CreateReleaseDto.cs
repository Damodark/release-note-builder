namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for creating a new release
/// </summary>
public class CreateReleaseDto
{
    public int BuildId { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
