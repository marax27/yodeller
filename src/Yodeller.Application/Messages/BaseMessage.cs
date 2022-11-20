namespace Yodeller.Application.Messages;

public abstract record BaseMessage
{
    public abstract void Invoke(IDownloadRequestsRepository repository);
}
