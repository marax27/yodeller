namespace Yodeller.Application.Models;

public record DownloadRequest(
    string Id,
    DateTime RequestedTime,
    string MediaLocator,
    bool AudioOnly,
    IReadOnlyCollection<string> SubtitlePatterns,
    ICollection<HistoryEntry> History,
    DownloadRequestStatus Status
);
