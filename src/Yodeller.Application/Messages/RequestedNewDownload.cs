using Yodeller.Application.Models;

namespace Yodeller.Application.Messages;

public record RequestedNewDownload(DownloadRequest Request) : BaseMessage
{
    public override void Invoke(IDownloadRequestsRepository repository)
    {
        repository.Add(Request);
    }
}
