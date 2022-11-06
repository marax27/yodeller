using Yodeller.Application.Models;

namespace Yodeller.Application.Messages;

public abstract record BaseMessage
{
    public abstract void Invoke(IDownloadRequestsRepository repository);
}

public record RequestedNewDownload(DownloadRequest Request) : BaseMessage
{
    public override void Invoke(IDownloadRequestsRepository repository)
    {
        repository.Add(Request);
    }
}
