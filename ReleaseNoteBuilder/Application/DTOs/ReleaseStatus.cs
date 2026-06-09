namespace ReleaseNoteBuilder.Application.DTOs;

/// <summary>
/// Enum representing the status of a release
/// </summary>
public enum ReleaseStatus
{
    Draft,
    PendingApproval,
    Approved,
    Deployed,
    Rejected,
    Failed
}