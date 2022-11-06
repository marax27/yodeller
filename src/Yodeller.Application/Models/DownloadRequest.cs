namespace Yodeller.Application.Models;

public enum DownloadRequestStatus
{
    New
}

public record DownloadRequest(
    string Id,
    DateTime RequestedTime,
    string MediaLocator,
    DownloadRequestStatus Status
);
