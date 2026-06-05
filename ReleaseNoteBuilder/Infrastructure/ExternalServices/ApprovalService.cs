using System;
using System.Threading.Tasks;

namespace ReleaseNoteBuilder.Infrastructure.ExternalServices;

/// <summary>
/// Service for handling approval workflows
/// </summary>
public class ApprovalService
{
    // TODO: Implement approval logic
    // This is a placeholder for future approval workflow

    public Task<bool> RequestApprovalAsync(int releaseId, string approver)
    {
        throw new NotImplementedException("Approval workflow not yet implemented");
    }

    public Task<bool> ApproveReleaseAsync(int releaseId, string approver)
    {
        throw new NotImplementedException("Approval workflow not yet implemented");
    }
}
