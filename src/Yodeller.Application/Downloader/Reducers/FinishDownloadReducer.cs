using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Downloader.Reducers;

public record FinishDownloadReducer(
    string RequestId,
    bool SuccessfulDownload,
    DateTime EndTime
) : IStateReducer<DownloadRequestsState>
{
    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var newStatus = SuccessfulDownload
            ? DownloadRequestStatus.Completed
            : DownloadRequestStatus.Failed;

        var historyEntry = new HistoryEntry(
            SuccessfulDownload ? "Completed." : "Failed.",
            EndTime
        );

        var requestToFinish = oldState.Requests
            .Single(request => request.Id == RequestId);

        var updatedRequest = requestToFinish with
        {
            Status = newStatus,
            History = requestToFinish.History.Append(historyEntry).ToList()
        };

        var newRequests = oldState.Requests
            .Select(request => request.Id == RequestId ? updatedRequest : request)
            .ToList();

        var alteredRequestIds = oldState.AlteredRequestIds.Append(RequestId).ToList();

        return new(newRequests, alteredRequestIds);
    }
}
