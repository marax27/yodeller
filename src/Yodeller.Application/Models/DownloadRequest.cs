namespace Yodeller.Application.Models;

public enum DownloadRequestStatus
{
    New,
    InProgress,
    Completed,
    Failed
}

public record DownloadRequest(
    string Id,
    DateTime RequestedTime,
    string MediaLocator,
    bool AudioOnly,
    DownloadRequestStatus Status
);
