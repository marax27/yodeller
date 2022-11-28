using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.CancelRequest;

public record CancelRequestReducer(string RequestId) : IStateReducer<DownloadRequestsState>
{
    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var newRequests = oldState.Requests
            .Select(request => request.Id == RequestId ? Cancel(request) : request)
            .ToList();

        var alteredRequestIds = oldState.AlteredRequestIds.Append(RequestId).ToList();

        return new(newRequests, alteredRequestIds);
    }

    private static DownloadRequest Cancel(DownloadRequest request)
    {
        if (request.Status == DownloadRequestStatus.New)
            return request with { Status = DownloadRequestStatus.Cancelled };
        else
            return request;
    }
}
