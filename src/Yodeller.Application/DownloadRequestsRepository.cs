using Yodeller.Application.Models;

namespace Yodeller.Application;

public interface IDownloadRequestsRepository
{
    void Add(DownloadRequest newRequest);

    DownloadRequest FindById(string id);

    IEnumerable<DownloadRequest> FindByStatus(DownloadRequestStatus status);
}

public class DownloadRequestsRepository : IDownloadRequestsRepository
{
    private readonly Dictionary<string, DownloadRequest> _requests = new();

    public void Add(DownloadRequest newRequest)
    {
        _requests.Add(newRequest.Id, newRequest);
    }

    public DownloadRequest FindById(string id) => _requests[id];

    public IEnumerable<DownloadRequest> FindByStatus(DownloadRequestStatus status) => _requests
        .Where(kvp => kvp.Value.Status == status)
        .Select(kvp => kvp.Value);
}
