namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for creating a new Work Item
/// </summary>
public class CreateWorkItemDto
{
    public int BuildId { get; set; }
    public string AdoWorkItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}