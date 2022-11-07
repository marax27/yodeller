using System.Collections.Concurrent;
using Yodeller.Application.Models;

namespace Yodeller.Application;

public interface IDownloadRequestsRepository
{
    void Add(DownloadRequest newRequest);

    void UpdateStatus(string id, DownloadRequestStatus newStatus);

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

    public void UpdateStatus(string id, DownloadRequestStatus newStatus)
    {
        var oldRequest = FindById(id);
        _requests[id] = oldRequest with { Status = newStatus };
    }

    public DownloadRequest FindById(string id) => _requests[id];

    public IEnumerable<DownloadRequest> FindByStatus(DownloadRequestStatus status) => _requests
        .Where(kvp => kvp.Value.Status == status)
        .Select(kvp => kvp.Value);
}
