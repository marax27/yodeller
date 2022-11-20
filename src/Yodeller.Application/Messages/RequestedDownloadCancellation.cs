namespace Yodeller.Application.Messages;

public record RequestedDownloadCancellation(string RequestId) : BaseMessage
{
    public override void Invoke(IDownloadRequestsRepository repository)
    {
        try
        {
            repository.TryCancel(RequestId);
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Request ID '{RequestId}' not found in the repository.");
        }
    }
}
