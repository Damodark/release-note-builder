using System;

namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// Data Transfer Object for Work Item
/// </summary>
public class WorkItemDto
{
    public int Id { get; set; }
    public string AdoWorkItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}