using Yodeller.Application.Downloader;

namespace Yodeller.Application.Ports;

public interface IMediaDownloader
{
    Task<bool> Download(DownloadProcessSpecification what);
}
