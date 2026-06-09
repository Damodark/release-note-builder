namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for updating a Work Item
/// </summary>
public class UpdateWorkItemDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
}