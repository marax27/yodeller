using Yodeller.Application.Models;

namespace Yodeller.Application.Messages;

public record RequestedDownloadCancellation(string RequestId) : BaseMessage
{
    public override void Invoke(IDownloadRequestsRepository repository)
    {
        try
        {
            var currentStatus = repository.FindById(RequestId).Status;
            if (currentStatus == DownloadRequestStatus.New)
            {
                repository.Cancel(RequestId);
            }
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Request ID '{RequestId}' not found in the repository.");
        }
    }
}
