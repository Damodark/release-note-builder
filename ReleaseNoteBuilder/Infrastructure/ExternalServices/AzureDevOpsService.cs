using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReleaseNoteBuilder.Infrastructure.ExternalServices;

/// <summary>
/// Service for interacting with Azure DevOps
/// </summary>
public class AzureDevOpsService
{
    // TODO: Implement Azure DevOps API integration
    // This is a placeholder for future ADO integration

    public Task<object> GetBuildDetailsAsync(int buildId)
    {
        throw new NotImplementedException("Azure DevOps integration not yet implemented");
    }

    public Task<List<object>> GetWorkItemsAsync(int buildId)
    {
        throw new NotImplementedException("Azure DevOps integration not yet implemented");
    }
}
