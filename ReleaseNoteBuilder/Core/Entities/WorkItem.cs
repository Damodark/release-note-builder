using System;

namespace ReleaseNoteBuilder.Core.Entities;

/// <summary>
/// Domain entity representing a Work Item
/// </summary>
public class WorkItem
{
    public int Id { get; set; }
    public string AdoWorkItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;  // High, Medium, Low
    public string Status { get; set; } = string.Empty;    // Active, Resolved, Closed
    public int BuildId { get; set; }
    public Build? Build { get; set; }
    public int ReleaseId { get; set; }
    public Release? Release { get; set; }
}