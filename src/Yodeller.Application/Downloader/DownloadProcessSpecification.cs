namespace Yodeller.Application.Downloader;

public record DownloadProcessSpecification(
    string MediaLocator,
    bool AudioOnly
);
