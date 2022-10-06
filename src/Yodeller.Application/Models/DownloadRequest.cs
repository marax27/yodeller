namespace Yodeller.Application.Models;

public enum DownloadRequestStatus
{
    New
}

public record DownloadRequest(
    string Id,
    DateTimeOffset RequestedTime,
    string MediaId,
    DownloadRequestStatus Status
);
