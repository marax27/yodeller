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

public record ClearFinishedRequests : BaseMessage
{
    public override void Invoke(IDownloadRequestsRepository repository)
    {
        var statusesToRemove = new[]
        {
            DownloadRequestStatus.Cancelled,
            DownloadRequestStatus.Completed,
            DownloadRequestStatus.Failed,
        };

        var idsToRemove = statusesToRemove
            .SelectMany(repository.FindByStatus)
            .Select(request => request.Id)
            .ToArray();

        repository.DeleteMany(idsToRemove);
    }
}
