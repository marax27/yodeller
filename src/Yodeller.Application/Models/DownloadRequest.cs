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
    DownloadRequestStatus Status
);
