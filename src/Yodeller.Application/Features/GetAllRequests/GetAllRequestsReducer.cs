using System.Diagnostics;
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
            .Select(Map)
            .ToArray();

        return new(requests);
    }

    private static GetAllRequestsQuery.DownloadRequestDto Map(DownloadRequest model) => new(
        model.Id,
        model.MediaLocator,
        model.AudioOnly,
        MapHistory(model.History),
        model.Status switch
        {
            DownloadRequestStatus.New => "New",
            DownloadRequestStatus.Completed => "Completed",
            DownloadRequestStatus.Failed => "Failed",
            DownloadRequestStatus.InProgress => "In progress",
            DownloadRequestStatus.Cancelled => "Cancelled",
            _ => throw new UnreachableException("Unsupported request status.")
        }
    );

    private static IReadOnlyCollection<GetAllRequestsQuery.HistoryEntryDto> MapHistory(IEnumerable<HistoryEntry> entries)
    {
        return entries
            .Select(entry => new GetAllRequestsQuery.HistoryEntryDto(entry.Description, entry.DateTime))
            .ToArray();
    }
}
