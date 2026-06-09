using System.Collections.Generic;
using ReleaseNoteBuilder.Application.DTOs;

namespace ReleaseNoteBuilder.Application.Interfaces;

/// <summary>
/// Interface for generating release documents
/// </summary>
public interface IReleaseDocumentService
{
    byte[] GenerateReleaseDocument(
        string projectName,
        string environment,
        BuildDto projectBuild,
        BuildDto configBuild,
        List<WorkItemDto> bugs,
        List<WorkItemDto> pbis,
        string requestedBy = "Admin User",
        string approvedBy = "Release Management");
}