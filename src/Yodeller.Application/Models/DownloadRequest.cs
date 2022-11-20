namespace Yodeller.Application.Models;

public enum DownloadRequestStatus
{
    New,
    InProgress,
    Completed,
    Failed,
    Cancelled
}

public record HistoryEntry(
    string Description,
    DateTime DateTime
);

public record DownloadRequest(
    string Id,
    DateTime RequestedTime,
    string MediaLocator,
    bool AudioOnly,
    IReadOnlyCollection<string> SubtitlePatterns,
    ICollection<HistoryEntry> History,
    DownloadRequestStatus Status
);
