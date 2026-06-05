namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for updating an existing release
/// </summary>
public class UpdateReleaseDto
{
    public int Id { get; set; }
    public int BuildId { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
