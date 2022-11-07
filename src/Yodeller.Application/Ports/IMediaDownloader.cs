using Yodeller.Application.Downloader;

namespace Yodeller.Application.Ports;

public interface IMediaDownloader
{
    bool Download(DownloadProcessSpecification what);
}
