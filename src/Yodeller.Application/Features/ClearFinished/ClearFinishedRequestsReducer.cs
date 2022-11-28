using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.ClearFinished;

public record ClearFinishedRequestsReducer : IStateReducer<DownloadRequestsState>
{
    private static readonly IReadOnlyCollection<DownloadRequestStatus> StatusesToRemove = new[]
    {
        DownloadRequestStatus.Cancelled,
        DownloadRequestStatus.Completed,
        DownloadRequestStatus.Failed,
    };

    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var newRequests = oldState.Requests
            .Where(request => !StatusesToRemove.Contains(request.Status))
            .ToList();

        return oldState with { Requests = newRequests };
    }
}
