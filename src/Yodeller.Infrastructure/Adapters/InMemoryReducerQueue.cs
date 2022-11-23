using Core.Shared.StateManagement;
using System.Collections.Concurrent;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Infrastructure.Adapters;

public class InMemoryReducerQueue : IMessageProducer<IStateReducer<DownloadRequestsState>>, IMessageConsumer<IStateReducer<DownloadRequestsState>>
{
    private readonly ConcurrentQueue<IStateReducer<DownloadRequestsState>> _requests = new();

    public void Produce(IStateReducer<DownloadRequestsState> message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        _requests.Enqueue(message);
    }

    public bool TryConsume(out IStateReducer<DownloadRequestsState> message)
    {
        var result = _requests.TryDequeue(out message!);
        return result;
    }
}
