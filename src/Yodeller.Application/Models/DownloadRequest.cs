namespace Yodeller.Application.Models;

public enum DownloadRequestStatus
{
    New,
    InProgress,
    Completed,
    Failed,
    Cancelled
}

public record DownloadRequest(
    string Id,
    DateTime RequestedTime,
    string MediaLocator,
    bool AudioOnly,
    IReadOnlyCollection<string> SubtitlePatterns,
    DownloadRequestStatus Status
);
