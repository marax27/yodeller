using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Downloader.Reducers;

public class LockRequestForDownloadReducer : IStateReducer<DownloadRequestsState>
{
    private readonly DateTime _downloadStartTime;
    private readonly Action<DownloadRequest?> _callback;

    public LockRequestForDownloadReducer(DateTime downloadStartTime, Action<DownloadRequest?> callback)
    {
        _downloadStartTime = downloadStartTime;
        _callback = callback;
    }

    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var selectedRequest = oldState.Requests
            .Where(request => request.Status == DownloadRequestStatus.New)
            .MinBy(request => request.RequestedTime);

        if (selectedRequest is null)
        {
            _callback(null);
            return oldState;
        }
        else
        {
            var updatedRequest = selectedRequest with
            {
                Status = DownloadRequestStatus.InProgress,
                History = selectedRequest.History.Append(new("Download started.", _downloadStartTime)).ToList()
            };

            var newRequests = oldState.Requests
                .Select(request => request.Id == updatedRequest.Id ? updatedRequest : request)
                .ToList();

            var alteredRequestIds = oldState.AlteredRequestIds.Append(updatedRequest.Id).ToList();

            _callback(updatedRequest);
            return new(newRequests, alteredRequestIds);
        }
    }
}
