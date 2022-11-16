namespace Yodeller.Application.Downloader;

public record DownloadProcessSpecification(
    IReadOnlyCollection<string> SubtitlePatterns,
    string MediaLocator,
    bool AudioOnly
);
