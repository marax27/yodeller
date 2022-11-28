using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.GetAllRequests;

public class GetAllRequestsReducer
    : ReadCallbackStateReducer<DownloadRequestsState, GetAllRequestsQuery.Result>
{
    private static readonly DownloadRequestStatus[] Statuses = new[]
    {
        DownloadRequestStatus.InProgress,
        DownloadRequestStatus.Failed,
        DownloadRequestStatus.New,
        DownloadRequestStatus.Completed,
        DownloadRequestStatus.Cancelled
    };

    public GetAllRequestsReducer(Action<GetAllRequestsQuery.Result> callback)
        : base(callback) { }

    protected override GetAllRequestsQuery.Result Compute(DownloadRequestsState state)
    {
        var requests = state.Requests
            .Where(request => Statuses.Contains(request.Status))
            .Select(DownloadRequestMapper.Map)
            .ToArray();

        return new(requests);
    }
}
