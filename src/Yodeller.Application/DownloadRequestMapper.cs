using System.Diagnostics;
using Yodeller.Application.Features.GetAllRequests;
using Yodeller.Application.Models;

namespace Yodeller.Application;

public class DownloadRequestMapper
{
    public static GetAllRequestsQuery.DownloadRequestDto Map(DownloadRequest model) => new(
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
