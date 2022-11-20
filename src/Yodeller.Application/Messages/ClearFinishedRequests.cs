using Yodeller.Application.Models;

namespace Yodeller.Application.Messages;

public record ClearFinishedRequests : BaseMessage
{
    private static readonly IReadOnlyCollection<DownloadRequestStatus> StatusesToRemove = new[]
    {
        DownloadRequestStatus.Cancelled,
        DownloadRequestStatus.Completed,
        DownloadRequestStatus.Failed,
    };

    public override void Invoke(IDownloadRequestsRepository repository)
    {
        var idsToRemove = StatusesToRemove
            .SelectMany(repository.FindByStatus)
            .Select(request => request.Id)
            .ToArray();

        repository.DeleteMany(idsToRemove);
    }
}
