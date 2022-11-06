using System.Collections.Concurrent;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class InMemoryMessageProducer : IMessageProducer
{
    private readonly ConcurrentBag<DownloadRequest> _requests = new();

    public void Produce(DownloadRequest request)
    {
        _requests.Add(request);
    }
}
