using System;
using System.Collections.Generic;
using ReleaseNoteBuilder.Core.Entities;

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
    public ReleaseStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? DeployedAt { get; set; }
    public List<WorkItemDto> WorkItems { get; set; } = new();
}
