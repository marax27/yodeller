using System.Collections.Concurrent;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class InMemoryRequestRepository : IRequestRepository
{
    private readonly ConcurrentBag<DownloadRequest> _requests = new();

    public void Add(DownloadRequest request)
    {
        _requests.Add(request);
    }
}
