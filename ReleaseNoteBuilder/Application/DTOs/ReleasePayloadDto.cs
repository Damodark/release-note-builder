using System;
using System.Collections.Generic;

namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for complete release information to be sent to the next page
/// </summary>
public class ReleasePayloadDto
{
    public string Project { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public BuildDto ProjectBuild { get; set; }
    public BuildDto ConfigBuild { get; set; }
    public List<WorkItemDto> Bugs { get; set; } = new();
    public List<WorkItemDto> PBIs { get; set; } = new();
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}