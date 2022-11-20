using System.Collections.Concurrent;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application;

public interface IDownloadRequestsRepository
{
    void Add(DownloadRequest newRequest);

    void Cancel(string id);

    void MarkDownloadInProgress(string id, DateTime downloadStartTime);

    void MarkDownloadEnd(string id, bool successful, DateTime downloadEndTime);

    DownloadRequest FindById(string id);

    IEnumerable<DownloadRequest> FindByStatus(DownloadRequestStatus status);
}

public class DownloadRequestsRepository : IDownloadRequestsRepository
{
    private readonly ConcurrentDictionary<string, DownloadRequest> _requests = new();

    public void Add(DownloadRequest newRequest)
    {
        if (!_requests.TryAdd(newRequest.Id, newRequest))
            throw new ArgumentException($"Duplicate request ID: '{newRequest.Id}'");
    }

    public void Cancel(string id)
    {
        var oldRequest = FindById(id);
        _requests[id] = oldRequest with { Status = DownloadRequestStatus.Cancelled };
    }

    public void MarkDownloadInProgress(string id, DateTime downloadStartTime)
    {
        var oldRequest = FindById(id);
        var historyEntry = new HistoryEntry("Download started.", downloadStartTime);
        _requests[id] = oldRequest with
        {
            Status = DownloadRequestStatus.InProgress,
            History = oldRequest.History.Concat(new[] { historyEntry }).ToArray()
        };
    }

    public void MarkDownloadEnd(string id, bool successful, DateTime downloadEndTime)
    {
        var oldRequest = FindById(id);
        var newStatus = successful
            ? DownloadRequestStatus.Completed
            : DownloadRequestStatus.Failed;
        var historyDescription = successful
            ? "Completed."
            : "Failed.";
        var historyEntry = new HistoryEntry(historyDescription, downloadEndTime);
        _requests[id] = oldRequest with
        {
            Status = newStatus,
            History = oldRequest.History.Concat(new[] { historyEntry }).ToArray()
        };
    }

    public DownloadRequest FindById(string id) => _requests[id];

    public IEnumerable<DownloadRequest> FindByStatus(DownloadRequestStatus status) => _requests
        .Where(kvp => kvp.Value.Status == status)
        .Select(kvp => kvp.Value);
}
