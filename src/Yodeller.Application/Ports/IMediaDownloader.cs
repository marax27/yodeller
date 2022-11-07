namespace Yodeller.Application.Ports;

public interface IMediaDownloader
{
    bool Download(string mediaLocator);
}
