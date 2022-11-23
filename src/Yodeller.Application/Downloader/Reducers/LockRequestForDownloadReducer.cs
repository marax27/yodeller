using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Downloader.Reducers;

public class LockRequestForDownloadReducer : IStateReducer<DownloadRequestsState>
{
    private readonly Action<DownloadRequest?> _callback;

    public LockRequestForDownloadReducer(Action<DownloadRequest?> callback)
    {
        _callback = callback;
    }

    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var selectedRequest = oldState.Requests
            .FirstOrDefault(request => request.Status == DownloadRequestStatus.New);

        if (selectedRequest is null)
        {
            _callback(null);
            return oldState;
        }
        else
        {
            var updatedRequest = selectedRequest with { Status = DownloadRequestStatus.InProgress };

            var newRequests = oldState.Requests
                .Select(request => request.Id == updatedRequest.Id ? updatedRequest : request)
                .ToList();

            _callback(updatedRequest);
            return new(newRequests);
        }
    }
}
