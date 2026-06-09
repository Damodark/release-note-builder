using System;

namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// DTO for updating a Build
/// </summary>
public class UpdateBuildDto
{
    public int Id { get; set; }
    public string? Status { get; set; }
    public string? Branch { get; set; }
    public DateTime? CompletedDate { get; set; }
}